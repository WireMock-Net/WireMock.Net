using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

internal class Transformer : ITransformer
{
    private static readonly Type[] SupportedTypes = { typeof(bool), typeof(long), typeof(int), typeof(double), typeof(Guid), typeof(DateTime), typeof(TimeSpan), typeof(Uri) };

    private readonly JsonSerializer _jsonSerializer;
    private readonly ITransformerContextFactory _factory;

    public Transformer(WireMockServerSettings settings, ITransformerContextFactory factory)
    {
        _factory = Guard.NotNull(factory);

        _jsonSerializer = new JsonSerializer
        {
            Culture = Guard.NotNull(settings).Culture
        };
    }

    public IBodyData? TransformBody(
        IMapping mapping,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage,
        IBodyData? bodyData,
        ReplaceNodeOptions options)
    {
        var (transformerContext, model) = Create(mapping, originalRequestMessage, originalResponseMessage);

        IBodyData? newBodyData = null;
        if (bodyData?.DetectedBodyType != null)
        {
            newBodyData = TransformBodyData(transformerContext, options, model, bodyData, false);
        }

        return newBodyData;
    }

    public IDictionary<string, WireMockList<string>> TransformHeaders(
        IMapping mapping,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage,
        IDictionary<string, WireMockList<string>>? headers
    )
    {
        var (transformerContext, model) = Create(mapping, originalRequestMessage, originalResponseMessage);

        return TransformHeaders(transformerContext, model, headers);
    }

    public string TransformString(
        IMapping mapping,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage,
        string? value
    )
    {
        if (value is null)
        {
            return string.Empty;
        }

        var (transformerContext, model) = Create(mapping, originalRequestMessage, originalResponseMessage);
        return transformerContext.ParseAndRender(value, model);
    }

    public ResponseMessage Transform(IMapping mapping, IRequestMessage requestMessage, IResponseMessage original, bool useTransformerForBodyAsFile, ReplaceNodeOptions options)
    {
        var responseMessage = new ResponseMessage();

        var (transformerContext, model) = Create(mapping, requestMessage, null);

        if (original.BodyData?.DetectedBodyType != null)
        {
            responseMessage.BodyData = TransformBodyData(transformerContext, options, model, original.BodyData, useTransformerForBodyAsFile);

            if (original.BodyData.DetectedBodyType is BodyType.String or BodyType.FormUrlEncoded)
            {
                responseMessage.BodyOriginal = original.BodyData.BodyAsString;
            }
        }

        responseMessage.FaultType = original.FaultType;
        responseMessage.FaultPercentage = original.FaultPercentage;

        responseMessage.Headers = TransformHeaders(transformerContext, model, original.Headers);

        switch (original.StatusCode)
        {
            case int statusCodeAsInteger:
                responseMessage.StatusCode = statusCodeAsInteger;
                break;

            case string statusCodeAsString:
                responseMessage.StatusCode = transformerContext.ParseAndRender(statusCodeAsString, model);
                break;
        }

        return responseMessage;
    }

    private (ITransformerContext TransformerContext, TransformModel Model) Create(IMapping mapping, IRequestMessage request, IResponseMessage? response)
    {
        return (_factory.Create(), new TransformModel
        {
            mapping = mapping,
            request = request,
            response = response,
            data = mapping.Data ?? new { }
        });
    }

    private IBodyData? TransformBodyData(ITransformerContext transformerContext, ReplaceNodeOptions options, TransformModel model, IBodyData original, bool useTransformerForBodyAsFile)
    {
        switch (original.DetectedBodyType)
        {
            case BodyType.Json:
                return TransformBodyAsJson(transformerContext, options, model, original);

            case BodyType.File:
                return TransformBodyAsFile(transformerContext, model, original, useTransformerForBodyAsFile);

            case BodyType.String:
            case BodyType.FormUrlEncoded:
                return TransformBodyAsString(transformerContext, model, original);

            default:
                return null;
        }
    }

    private static IDictionary<string, WireMockList<string>> TransformHeaders(ITransformerContext transformerContext, TransformModel model, IDictionary<string, WireMockList<string>>? original)
    {
        if (original == null)
        {
            return new Dictionary<string, WireMockList<string>>();
        }

        var newHeaders = new Dictionary<string, WireMockList<string>>();
        foreach (var header in original)
        {
            var headerKey = transformerContext.ParseAndRender(header.Key, model);
            var templateHeaderValues = header.Value.Select(text => transformerContext.ParseAndRender(text, model)).ToArray();

            newHeaders.Add(headerKey, new WireMockList<string>(templateHeaderValues));
        }

        return newHeaders;
    }

    private IBodyData TransformBodyAsJson(ITransformerContext transformerContext, ReplaceNodeOptions options, object model, IBodyData original)
    {
        JToken? jToken = null;
        switch (original.BodyAsJson)
        {
            case JObject bodyAsJObject:
                jToken = bodyAsJObject.DeepClone();
                WalkNode(transformerContext, options, jToken, model);
                break;

            case JArray bodyAsJArray:
                jToken = bodyAsJArray.DeepClone();
                WalkNode(transformerContext, options, jToken, model);
                break;

            case Array bodyAsArray:
                jToken = JArray.FromObject(bodyAsArray, _jsonSerializer);
                WalkNode(transformerContext, options, jToken, model);
                break;

            case string bodyAsString:
                jToken = ReplaceSingleNode(transformerContext, options, bodyAsString, model);
                break;

            case not null:
                jToken = JObject.FromObject(original.BodyAsJson, _jsonSerializer);
                WalkNode(transformerContext, options, jToken, model);
                break;
        }

        return new BodyData
        {
            Encoding = original.Encoding,
            DetectedBodyType = original.DetectedBodyType,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            BodyAsJson = jToken
        };
    }

    private JToken ReplaceSingleNode(ITransformerContext transformerContext, ReplaceNodeOptions options, string stringValue, object model)
    {
        string transformedString = transformerContext.ParseAndRender(stringValue, model);

        if (!string.Equals(stringValue, transformedString))
        {
            const string property = "_";
            JObject dummy = JObject.Parse($"{{ \"{property}\": null }}");
            if (dummy[property] == null)
            {
                // TODO: check if just returning null is fine
                return string.Empty;
            }

            JToken node = dummy[property]!;

            ReplaceNodeValue(options, node, transformedString);

            return dummy[property]!;
        }

        return stringValue;
    }

    private void WalkNode(ITransformerContext transformerContext, ReplaceNodeOptions options, JToken node, object model)
    {
        switch (node.Type)
        {
            case JTokenType.Object:
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (var child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(transformerContext, options, child.Value, model);
                }
                break;

            case JTokenType.Array:
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (var child in node.Children().ToArray())
                {
                    WalkNode(transformerContext, options, child, model);
                }
                break;

            case JTokenType.String:
                // In case of string, try to transform the value.
                var stringValue = node.Value<string>();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return;
                }

                var transformed = transformerContext.ParseAndEvaluate(stringValue!, model);
                if (!Equals(stringValue, transformed))
                {
                    ReplaceNodeValue(options, node, transformed);
                }
                break;
        }
    }

    // ReSharper disable once UnusedParameter.Local
    private void ReplaceNodeValue(ReplaceNodeOptions options, JToken node, object? transformedValue)
    {
        switch (transformedValue)
        {
            case JValue jValue:
                node.Replace(jValue);
                return;

            case string transformedString:
                if (options != ReplaceNodeOptions.KeepString && TryConvert(transformedString, out var convertedFromStringValue))
                {
                    node.Replace(JToken.FromObject(convertedFromStringValue, _jsonSerializer));
                }
                else
                {
                    node.Replace(ParseAsJObject(transformedString));
                }
                break;

            case WireMockList<string> strings:
                switch (strings.Count)
                {
                    case 1:
                        node.Replace(ParseAsJObject(strings[0]));
                        return;

                    case > 1:
                        node.Replace(JToken.FromObject(strings.ToArray(), _jsonSerializer));
                        return;
                }
                break;

            case { }:
                if (TryConvert(transformedValue, out var convertedValue))
                {
                    node.Replace(JToken.FromObject(convertedValue, _jsonSerializer));
                }
                return;

            default: // It's null, skip it. Maybe remove it ?
                return;
        }
    }

    private static JToken ParseAsJObject(string stringValue)
    {
        return JsonUtils.TryParseAsJObject(stringValue, out var parsedAsjObject) ? parsedAsjObject : stringValue;
    }

    private static bool TryConvert(object? transformedValue, [NotNullWhen(true)] out object? convertedValue)
    {
        foreach (var supportedType in SupportedTypes)
        {
            try
            {
                convertedValue = Convert.ChangeType(transformedValue, supportedType);
                return convertedValue is not null;
            }
            catch
            {
                // Ignore
            }
        }

        convertedValue = null;
        return false;
    }

    private static IBodyData TransformBodyAsString(ITransformerContext transformerContext, object model, IBodyData original)
    {
        return new BodyData
        {
            Encoding = original.Encoding,
            DetectedBodyType = original.DetectedBodyType,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            BodyAsString = transformerContext.ParseAndRender(original.BodyAsString!, model)
        };
    }

    private static IBodyData TransformBodyAsFile(ITransformerContext transformerContext, object model, IBodyData original, bool useTransformerForBodyAsFile)
    {
        string transformedBodyAsFilename = transformerContext.ParseAndRender(original.BodyAsFile!, model);

        if (!useTransformerForBodyAsFile)
        {
            return new BodyData
            {
                DetectedBodyType = original.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
                BodyAsFile = transformedBodyAsFilename
            };
        }

        string text = transformerContext.FileSystemHandler.ReadResponseBodyAsString(transformedBodyAsFilename);
        return new BodyData
        {
            DetectedBodyType = BodyType.String,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            BodyAsString = transformerContext.ParseAndRender(text, model),
            BodyAsFile = transformedBodyAsFilename
        };
    }
}
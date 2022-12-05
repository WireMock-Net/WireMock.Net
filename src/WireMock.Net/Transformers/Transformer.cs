using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

internal class Transformer : ITransformer
{
    private readonly JsonSerializer _jsonSerializer;
    private readonly WireMockServerSettings _settings;
    private readonly ITransformerContextFactory _factory;

    public Transformer(WireMockServerSettings settings, ITransformerContextFactory factory)
    {
        _settings = Guard.NotNull(settings);
        _factory = Guard.NotNull(factory);

        _jsonSerializer = new JsonSerializer
        {
            Culture = _settings.Culture
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

            if (original.BodyData.DetectedBodyType == BodyType.String)
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
            response = response
        });
    }

    private IBodyData? TransformBodyData(ITransformerContext transformerContext, ReplaceNodeOptions options, TransformModel model, IBodyData original, bool useTransformerForBodyAsFile)
    {
        return original.DetectedBodyType switch
        {
            BodyType.Json => TransformBodyAsJson(transformerContext, options, model, original),
            BodyType.File => TransformBodyAsFile(transformerContext, model, original, useTransformerForBodyAsFile),
            BodyType.String => TransformBodyAsString(transformerContext, model, original),
            _ => null
        };
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

    private IBodyData TransformBodyAsJson(ITransformerContext handlebarsContext, ReplaceNodeOptions options, object model, IBodyData original)
    {
        JToken? jToken = null;
        switch (original.BodyAsJson)
        {
            case JObject bodyAsJObject:
                jToken = bodyAsJObject.DeepClone();
                WalkNode(handlebarsContext, options, jToken, model);
                break;

            case JArray bodyAsJArray:
                jToken = bodyAsJArray.DeepClone();
                WalkNode(handlebarsContext, options, jToken, model);
                break;

            case Array bodyAsArray:
                jToken = JArray.FromObject(bodyAsArray, _jsonSerializer);
                WalkNode(handlebarsContext, options, jToken, model);
                break;

            case string bodyAsString:
                jToken = ReplaceSingleNode(handlebarsContext, options, bodyAsString, model);
                break;

            case not null:
                jToken = JObject.FromObject(original.BodyAsJson, _jsonSerializer);
                WalkNode(handlebarsContext, options, jToken, model);
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

    private JToken ReplaceSingleNode(ITransformerContext handlebarsContext, ReplaceNodeOptions options, string stringValue, object model)
    {
        string transformedString = handlebarsContext.ParseAndRender(stringValue, model);

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

    private void WalkNode(ITransformerContext handlebarsContext, ReplaceNodeOptions options, JToken node, object model)
    {
        switch (node.Type)
        {
            case JTokenType.Object:
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (var child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(handlebarsContext, options, child.Value, model);
                }
                break;

            case JTokenType.Array:
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (var child in node.Children().ToArray())
                {
                    WalkNode(handlebarsContext, options, child, model);
                }
                break;

            case JTokenType.String:
                // In case of string, try to transform the value.
                var stringValue = node.Value<string>();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return;
                }

                var transformed = handlebarsContext.ParseAndEvaluate(stringValue!, model);
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
        if (transformedValue is string transformedString)
        {
            StringUtils.TryParseQuotedString(transformedString, out var result, out _);
            if (bool.TryParse(result, out var valueAsBoolean) || bool.TryParse(transformedString, out valueAsBoolean))
            {
                node.Replace(valueAsBoolean);
                return;
            }
        }

        if (transformedValue is { })
        {
            try
            {
                // Try to convert this string into a JsonObject
                //value = JToken.Parse(transformedValue);

                var value = JToken.FromObject(transformedValue, _jsonSerializer);
                node.Replace(value);
            }
            catch (JsonException)
            {
                // Ignore JsonException and just keep string value and convert to JToken
                //value = transformedString;
            }
        }
        else
        {
            //node.Remove(); // TODO
        }

        //try
        //{
        //    // Try to convert this string into a JsonObject
        //    //value = JToken.Parse(transformedValue);
        //    var value = JToken.FromObject(transformedValue);
        //    node.Replace(value);
        //}
        //catch (JsonException)
        //{
        //    // Ignore JsonException and just keep string value and convert to JToken
        //    //value = transformedString;
        //}

        //node.Replace(value);
    }

    private static IBodyData TransformBodyAsString(ITransformerContext handlebarsContext, object model, IBodyData original)
    {
        return new BodyData
        {
            Encoding = original.Encoding,
            DetectedBodyType = original.DetectedBodyType,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            BodyAsString = handlebarsContext.ParseAndRender(original.BodyAsString!, model)
        };
    }

    private static IBodyData TransformBodyAsFile(ITransformerContext handlebarsContext, object model, IBodyData original, bool useTransformerForBodyAsFile)
    {
        string transformedBodyAsFilename = handlebarsContext.ParseAndRender(original.BodyAsFile!, model);

        if (!useTransformerForBodyAsFile)
        {
            return new BodyData
            {
                DetectedBodyType = original.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
                BodyAsFile = transformedBodyAsFilename
            };
        }

        string text = handlebarsContext.FileSystemHandler.ReadResponseBodyAsString(transformedBodyAsFilename);
        return new BodyData
        {
            DetectedBodyType = BodyType.String,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            BodyAsString = handlebarsContext.ParseAndRender(text, model),
            BodyAsFile = transformedBodyAsFilename
        };
    }
}
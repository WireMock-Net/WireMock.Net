using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers
{
    internal class Transformer : ITransformer
    {
        private readonly ITransformerContextFactory _factory;

        public Transformer([NotNull] ITransformerContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public (IBodyData BodyData, IDictionary<string, WireMockList<string>> Headers) Transform(RequestMessage originalRequestMessage, ResponseMessage originalResponseMessage, IBodyData bodyData, IDictionary<string, WireMockList<string>> headers, ReplaceNodeOption option)
        {
            var transformerContext = _factory.Create();

            var model = new
            {
                request = originalRequestMessage,
                response = originalResponseMessage
            };

            IBodyData newBodyData = null;
            if (bodyData?.DetectedBodyType != null)
            {
                newBodyData = TransformBodyData(transformerContext, option, model, bodyData, false);
            }

            return (newBodyData, TransformHeaders(transformerContext, model, headers));
        }

        public ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile, ReplaceNodeOption option)
        {
            var transformerContext = _factory.Create();

            var responseMessage = new ResponseMessage();

            var model = new
            {
                request = requestMessage
            };

            if (original.BodyData?.DetectedBodyType != null)
            {
                responseMessage.BodyData = TransformBodyData(transformerContext, option, model, original.BodyData, useTransformerForBodyAsFile);

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

        private static IBodyData TransformBodyData(ITransformerContext transformerContext, ReplaceNodeOption option, object model, IBodyData original, bool useTransformerForBodyAsFile)
        {
            switch (original?.DetectedBodyType)
            {
                case BodyType.Json:
                    return TransformBodyAsJson(transformerContext, option, model, original);

                case BodyType.File:
                    return TransformBodyAsFile(transformerContext, model, original, useTransformerForBodyAsFile);

                case BodyType.String:
                    return TransformBodyAsString(transformerContext, model, original);

                default:
                    return null;
            }
        }

        private static IDictionary<string, WireMockList<string>> TransformHeaders(ITransformerContext transformerContext, object model, IDictionary<string, WireMockList<string>> original)
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

        private static IBodyData TransformBodyAsJson(ITransformerContext handlebarsContext, ReplaceNodeOption option, object model, IBodyData original)
        {
            JToken jToken;
            switch (original.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jToken = bodyAsJObject.DeepClone();
                    WalkNode(handlebarsContext, option, jToken, model);
                    break;

                case Array bodyAsArray:
                    jToken = JArray.FromObject(bodyAsArray);
                    WalkNode(handlebarsContext, option, jToken, model);
                    break;

                case string bodyAsString:
                    jToken = ReplaceSingleNode(handlebarsContext, option, bodyAsString, model);
                    break;

                default:
                    jToken = JObject.FromObject(original.BodyAsJson);
                    WalkNode(handlebarsContext, option, jToken, model);
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

        private static JToken ReplaceSingleNode(ITransformerContext handlebarsContext, ReplaceNodeOption option, string stringValue, object model)
        {
            string transformedString = handlebarsContext.ParseAndRender(stringValue, model);

            if (!string.Equals(stringValue, transformedString))
            {
                const string property = "_";
                JObject dummy = JObject.Parse($"{{ \"{property}\": null }}");
                JToken node = dummy[property];

                ReplaceNodeValue(option, node, transformedString);

                return dummy[property];
            }

            return stringValue;
        }

        private static void WalkNode(ITransformerContext handlebarsContext, ReplaceNodeOption option, JToken node, object model)
        {
            switch (node.Type)
            {
                case JTokenType.Object:
                    // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                    foreach (var child in node.Children<JProperty>().ToArray())
                    {
                        WalkNode(handlebarsContext, option, child.Value, model);
                    }
                    break;

                case JTokenType.Array:
                    // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                    foreach (var child in node.Children().ToArray())
                    {
                        WalkNode(handlebarsContext, option, child, model);
                    }
                    break;

                case JTokenType.String:
                    // In case of string, try to transform the value.
                    string stringValue = node.Value<string>();
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        return;
                    }

                    string transformedString = handlebarsContext.ParseAndRender(stringValue, model);
                    if (!string.Equals(stringValue, transformedString))
                    {
                        ReplaceNodeValue(option, node, transformedString);
                    }
                    break;
            }
        }

        private static void ReplaceNodeValue(ReplaceNodeOption option, JToken node, string stringValue)
        {
            if (option.HasFlag(ReplaceNodeOption.Bool) && bool.TryParse(stringValue, out bool valueAsBoolean))
            {
                node.Replace(valueAsBoolean);
                return;
            }

            if (option.HasFlag(ReplaceNodeOption.Integer) && int.TryParse(stringValue, out int valueAsInteger))
            {
                node.Replace(valueAsInteger);
                return;
            }

            if (option.HasFlag(ReplaceNodeOption.Long) && long.TryParse(stringValue, out long valueAsLong))
            {
                node.Replace(valueAsLong);
                return;
            }

            if (!JsonUtils.TryParseAsComplexObject(stringValue, out JToken value))
            {
                // Just keep string value and convert to JToken
                value = stringValue;
            }

            node.Replace(value);
        }

        private static IBodyData TransformBodyAsString(ITransformerContext handlebarsContext, object model, IBodyData original)
        {
            return new BodyData
            {
                Encoding = original.Encoding,
                DetectedBodyType = original.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
                BodyAsString = handlebarsContext.ParseAndRender(original.BodyAsString, model)
            };
        }

        private static IBodyData TransformBodyAsFile(ITransformerContext handlebarsContext, object model, IBodyData original, bool useTransformerForBodyAsFile)
        {
            string transformedBodyAsFilename = handlebarsContext.ParseAndRender(original.BodyAsFile, model);

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
}
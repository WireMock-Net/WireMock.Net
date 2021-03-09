using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers.Handlebars
{
    internal class Transformer : ITransformer
    {
        private readonly ITransformerContextFactory _factory;

        public Transformer([NotNull] ITransformerContextFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile)
        {
            var handlebarsContext = _factory.Create();

            var responseMessage = new ResponseMessage();

            var model = new { request = requestMessage };

            switch (original.BodyData?.DetectedBodyType)
            {
                case BodyType.Json:
                    TransformBodyAsJson(handlebarsContext, model, original, responseMessage);
                    break;

                case BodyType.File:
                    TransformBodyAsFile(handlebarsContext, model, original, responseMessage, useTransformerForBodyAsFile);
                    break;

                case BodyType.String:
                    responseMessage.BodyOriginal = original.BodyData.BodyAsString;
                    TransformBodyAsString(handlebarsContext, model, original, responseMessage);
                    break;
            }

            responseMessage.FaultType = original.FaultType;
            responseMessage.FaultPercentage = original.FaultPercentage;

            // Headers
            if (original.Headers != null)
            {
                var newHeaders = new Dictionary<string, WireMockList<string>>();
                foreach (var header in original.Headers)
                {
                    var headerKey = handlebarsContext.ParseAndRender(header.Key, model);
                    var templateHeaderValues = header.Value.Select(text => handlebarsContext.ParseAndRender(text, model)).ToArray();

                    newHeaders.Add(headerKey, new WireMockList<string>(templateHeaderValues));
                }

                responseMessage.Headers = newHeaders;
            }

            switch (original.StatusCode)
            {
                case int statusCodeAsInteger:
                    responseMessage.StatusCode = statusCodeAsInteger;
                    break;

                case string statusCodeAsString:
                    responseMessage.StatusCode = handlebarsContext.ParseAndRender(statusCodeAsString, model);
                    break;
            }

            return responseMessage;
        }

        private static void TransformBodyAsJson(ITransformerContext handlebarsContext, object model, ResponseMessage original, ResponseMessage responseMessage)
        {
            JToken jToken;
            switch (original.BodyData.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jToken = bodyAsJObject.DeepClone();
                    WalkNode(handlebarsContext, jToken, model);
                    break;

                case Array bodyAsArray:
                    jToken = JArray.FromObject(bodyAsArray);
                    WalkNode(handlebarsContext, jToken, model);
                    break;

                case string bodyAsString:
                    jToken = ReplaceSingleNode(handlebarsContext, bodyAsString, model);
                    break;

                default:
                    jToken = JObject.FromObject(original.BodyData.BodyAsJson);
                    WalkNode(handlebarsContext, jToken, model);
                    break;
            }

            responseMessage.BodyData = new BodyData
            {
                Encoding = original.BodyData.Encoding,
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsJson = jToken
            };
        }

        private static JToken ReplaceSingleNode(ITransformerContext handlebarsContext, string stringValue, object model)
        {
            string transformedString = handlebarsContext.ParseAndRender(stringValue, model) as string;

            if (!string.Equals(stringValue, transformedString))
            {
                const string property = "_";
                JObject dummy = JObject.Parse($"{{ \"{property}\": null }}");
                JToken node = dummy[property];

                ReplaceNodeValue(node, transformedString);

                return dummy[property];
            }

            return stringValue;
        }

        private static void WalkNode(ITransformerContext handlebarsContext, JToken node, object model)
        {
            if (node.Type == JTokenType.Object)
            {
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JProperty child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(handlebarsContext, child.Value, model);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JToken child in node.Children().ToArray())
                {
                    WalkNode(handlebarsContext, child, model);
                }
            }
            else if (node.Type == JTokenType.String)
            {
                // In case of string, try to transform the value.
                string stringValue = node.Value<string>();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return;
                }

                string transformedString = handlebarsContext.ParseAndRender(stringValue, model);
                if (!string.Equals(stringValue, transformedString))
                {
                    ReplaceNodeValue(node, transformedString);
                }
            }
        }

        private static void ReplaceNodeValue(JToken node, string stringValue)
        {
            if (bool.TryParse(stringValue, out bool valueAsBoolean))
            {
                node.Replace(valueAsBoolean);
                return;
            }

            JToken value;
            try
            {
                // Try to convert this string into a JsonObject
                value = JToken.Parse(stringValue);
            }
            catch (JsonException)
            {
                // Ignore JsonException and just keep string value and convert to JToken
                value = stringValue;
            }

            node.Replace(value);
        }

        private static void TransformBodyAsString(ITransformerContext handlebarsContext, object model, ResponseMessage original, ResponseMessage responseMessage)
        {
            responseMessage.BodyData = new BodyData
            {
                Encoding = original.BodyData.Encoding,
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsString = handlebarsContext.ParseAndRender(original.BodyData.BodyAsString, model)
            };
        }

        private void TransformBodyAsFile(ITransformerContext handlebarsContext, object model, ResponseMessage original, ResponseMessage responseMessage, bool useTransformerForBodyAsFile)
        {
            string transformedBodyAsFilename = handlebarsContext.ParseAndRender(original.BodyData.BodyAsFile, model);

            if (!useTransformerForBodyAsFile)
            {
                responseMessage.BodyData = new BodyData
                {
                    DetectedBodyType = original.BodyData.DetectedBodyType,
                    DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                    BodyAsFile = transformedBodyAsFilename
                };
            }
            else
            {
                string text = handlebarsContext.FileSystemHandler.ReadResponseBodyAsString(transformedBodyAsFilename);

                responseMessage.BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                    BodyAsString = handlebarsContext.ParseAndRender(text, model),
                    BodyAsFile = transformedBodyAsFilename
                };
            }
        }
    }
}
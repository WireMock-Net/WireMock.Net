using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Util;

namespace WireMock.Transformers
{
    internal static class ResponseMessageTransformer
    {
        private static readonly HandlebarsConfiguration HandlebarsConfiguration = new HandlebarsConfiguration
        {
            UnresolvedBindingFormatter = "{0}"
        };

        private static readonly IHandlebars HandlebarsContext = Handlebars.Create(HandlebarsConfiguration);

        static ResponseMessageTransformer()
        {
            HandlebarsHelpers.Register(HandlebarsContext);
        }

        public static ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original)
        {
            var responseMessage = new ResponseMessage { StatusCode = original.StatusCode };

            var template = new { request = requestMessage };

            switch (original.BodyData.DetectedBodyType)
            {
                case BodyType.Json:
                    TransformBodyAsJson(template, original, responseMessage);
                    break;

                case BodyType.File:
                    TransformBodyAsFile(template, original, responseMessage);
                    break;

                case BodyType.String:
                    responseMessage.BodyOriginal = original.BodyData.BodyAsString;
                    TransformBodyAsString(template, original, responseMessage);
                    break;
            }

            // Headers
            var newHeaders = new Dictionary<string, WireMockList<string>>();
            foreach (var header in original.Headers)
            {
                var templateHeaderKey = HandlebarsContext.Compile(header.Key);
                var templateHeaderValues = header.Value
                    .Select(HandlebarsContext.Compile)
                    .Select(func => func(template))
                    .ToArray();

                newHeaders.Add(templateHeaderKey(template), new WireMockList<string>(templateHeaderValues));
            }

            responseMessage.Headers = newHeaders;

            return responseMessage;
        }

        private static void TransformBodyAsJson(object template, ResponseMessage original, ResponseMessage responseMessage)
        {
            JToken jToken;
            switch (original.BodyData.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jToken = bodyAsJObject.DeepClone();
                    break;

                case Array bodyAsArray:
                    jToken = JArray.FromObject(bodyAsArray);
                    break;

                default:
                    jToken = JObject.FromObject(original.BodyData.BodyAsJson);
                    break;
            }

            WalkNode(jToken, template);

            responseMessage.BodyData = new BodyData
            {
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsJson = jToken
            };
        }

        private static void WalkNode(JToken node, object template)
        {
            if (node.Type == JTokenType.Object)
            {
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JProperty child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(child.Value, template);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JToken child in node.Children().ToArray())
                {
                    WalkNode(child, template);
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

                var templateForStringValue = HandlebarsContext.Compile(stringValue);
                string transformedString = templateForStringValue(template);
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

        private static void TransformBodyAsString(object template, ResponseMessage original, ResponseMessage responseMessage)
        {
            var templateBodyAsString = HandlebarsContext.Compile(original.BodyData.BodyAsString);

            responseMessage.BodyData = new BodyData
            {
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsString = templateBodyAsString(template)
            };
        }

        private static void TransformBodyAsFile(object template, ResponseMessage original, ResponseMessage responseMessage)
        {
            var templateBodyAsFile = HandlebarsContext.Compile(original.BodyData.BodyAsFile);

            responseMessage.BodyData = new BodyData
            {
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsFile = templateBodyAsFile(template)
            };
        }
    }
}

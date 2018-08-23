using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Util;

namespace WireMock.Transformers
{
    internal static class ResponseMessageTransformer
    {
        static ResponseMessageTransformer()
        {
            HandlebarsHelpers.Register();
        }

        public static ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original)
        {
            bool bodyIsJson = original.BodyAsJson != null;
            var responseMessage = new ResponseMessage { StatusCode = original.StatusCode };

            if (!bodyIsJson)
            {
                responseMessage.BodyOriginal = original.Body;
            }

            var template = new { request = requestMessage };

            if (!bodyIsJson)
            {
                TransformBodyAsString(template, original, responseMessage);
            }
            else
            {
                TransformBodyAsJson(template, original, responseMessage);
            }

            // Headers
            var newHeaders = new Dictionary<string, WireMockList<string>>();
            foreach (var header in original.Headers)
            {
                var templateHeaderKey = Handlebars.Compile(header.Key);
                var templateHeaderValues = header.Value
                    .Select(Handlebars.Compile)
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
            switch (original.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jToken = bodyAsJObject;
                    break;

                case Array bodyAsArray:
                    jToken = JArray.FromObject(bodyAsArray);
                    break;

                default:
                    jToken = JObject.FromObject(original.BodyAsJson);
                    break;
            }

            WalkNode(jToken, template);

            responseMessage.BodyAsJson = jToken;
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

                var templateForStringValue = Handlebars.Compile(stringValue);
                string transformedString = templateForStringValue(template);
                if (!string.Equals(stringValue, transformedString))
                {
                    JToken value;
                    try
                    {
                        // Try to convert this string into a real JsonObject
                        value = JToken.Parse(transformedString);
                    }
                    catch (JsonException)
                    {
                        // Ignore JsonException and just convert to JToken
                        value = transformedString;
                    }

                    node.Replace(value);
                }
            }
        }

        private static void TransformBodyAsString(object template, ResponseMessage original, ResponseMessage responseMessage)
        {
            var templateBody = Handlebars.Compile(original.Body);

            responseMessage.Body = templateBody(template);
        }
    }
}
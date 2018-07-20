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
            JObject jobject;
            switch (original.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jobject = bodyAsJObject;
                    break;

                default:
                    jobject = JObject.FromObject(original.BodyAsJson);
                    break;
            }

            WalkNode(jobject, template);

            responseMessage.BodyAsJson = jobject;
        }

        private static void WalkNode(JToken node, object template)
        {
            if (node.Type == JTokenType.Object)
            {
                // In case of Object, loop all children.
                foreach (JProperty child in node.Children<JProperty>())
                {
                    WalkNode(child.Value, template);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                // In case of Array, loop all items.
                foreach (JToken child in node.Children())
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
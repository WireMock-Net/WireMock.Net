using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Transformers.Scriban
{
    internal class ScribanTransformer : ITransformer
    {
        private readonly ITransformerContextFactory<ScribanContext> _factory;

        public ScribanTransformer([NotNull] ITransformerContextFactory<ScribanContext> factory)
        {
            Check.NotNull(factory, nameof(factory));

            _factory = factory;
        }

        public ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile)
        {
            var context = _factory.Create();

            var responseMessage = new ResponseMessage();

            var model = new { request = requestMessage };

            switch (original.BodyData?.DetectedBodyType)
            {
                case BodyType.Json:
                    TransformBodyAsJson(context, model, original, responseMessage);
                    break;

                case BodyType.File:
                    TransformBodyAsFile(context, model, original, responseMessage, useTransformerForBodyAsFile);
                    break;

                case BodyType.String:
                    responseMessage.BodyOriginal = original.BodyData.BodyAsString;
                    TransformBodyAsString(context, model, original, responseMessage);
                    break;
            }

            responseMessage.FaultType = original.FaultType;
            responseMessage.FaultPercentage = original.FaultPercentage;

            // Headers
            var newHeaders = new Dictionary<string, WireMockList<string>>();
            foreach (var header in original.Headers)
            {
                var templateHeaderKey = context.Parse(header.Key);

                var templateHeaderValues = header.Value
                    .Select(x => context.Parse(x))
                    .Select(t => t.Render(model))
                    .ToArray();

                newHeaders.Add(templateHeaderKey.Render(model), new WireMockList<string>(templateHeaderValues));
            }

            responseMessage.Headers = newHeaders;

            switch (original.StatusCode)
            {
                case int statusCodeAsInteger:
                    responseMessage.StatusCode = statusCodeAsInteger;
                    break;

                case string statusCodeAsString:
                    responseMessage.StatusCode = context.Parse(statusCodeAsString).Render(model);
                    break;
            }

            return responseMessage;
        }

        private static void TransformBodyAsJson(ScribanContext context, object model, ResponseMessage original, ResponseMessage responseMessage)
        {
            JToken jToken;
            switch (original.BodyData.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jToken = bodyAsJObject.DeepClone();
                    WalkNode(context, jToken, model);
                    break;

                case Array bodyAsArray:
                    jToken = JArray.FromObject(bodyAsArray);
                    WalkNode(context, jToken, model);
                    break;

                case string bodyAsString:
                    jToken = ReplaceSingleNode(context, bodyAsString, model);
                    break;

                default:
                    jToken = JObject.FromObject(original.BodyData.BodyAsJson);
                    WalkNode(context, jToken, model);
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

        private static JToken ReplaceSingleNode(ScribanContext context, string stringValue, object model)
        {
            string transformedString = context.Parse(stringValue).Render(model);
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

        private static void WalkNode(ScribanContext context, JToken node, object model)
        {
            if (node.Type == JTokenType.Object)
            {
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JProperty child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(context, child.Value, model);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JToken child in node.Children().ToArray())
                {
                    WalkNode(context, child, model);
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

                string transformedString = context.Parse(stringValue).Render(model);
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

        private static void TransformBodyAsString(ScribanContext context, object model, ResponseMessage original, ResponseMessage responseMessage)
        {
            responseMessage.BodyData = new BodyData
            {
                Encoding = original.BodyData.Encoding,
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsString = context.Parse(original.BodyData.BodyAsString).Render(model)
            };
        }

        private void TransformBodyAsFile(ScribanContext context, object model, ResponseMessage original, ResponseMessage responseMessage, bool useTransformerForBodyAsFile)
        {
            string transformedBodyAsFilename = context.Parse(original.BodyData.BodyAsFile).Render(model);

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
                string text = context.FileSystemHandler.ReadResponseBodyAsString(transformedBodyAsFilename);
                var templateBodyAsString = context.Parse(text);

                responseMessage.BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                    BodyAsString = templateBodyAsString.Render(model),
                    BodyAsFile = transformedBodyAsFilename
                };
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal class DotLiquidTransformer : IResponseMessageTransformer
    {
        private readonly ITransformerContext _transformerContext;

        public DotLiquidTransformer([NotNull] ITransformerContext transformerContext)
        {
            Check.NotNull(transformerContext, nameof(transformerContext));

            _transformerContext = transformerContext;
        }

        public ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile)
        {
            var responseMessage = new ResponseMessage();

            var hash = Hash.FromAnonymousObject(new { request = requestMessage });

            switch (original.BodyData?.DetectedBodyType)
            {
                case BodyType.Json:
                    TransformBodyAsJson(hash, original, responseMessage);
                    break;

                case BodyType.File:
                    TransformBodyAsFile(hash, original, responseMessage, useTransformerForBodyAsFile);
                    break;

                case BodyType.String:
                    responseMessage.BodyOriginal = original.BodyData.BodyAsString;
                    TransformBodyAsString(hash, original, responseMessage);
                    break;
            }

            responseMessage.FaultType = original.FaultType;
            responseMessage.FaultPercentage = original.FaultPercentage;

            // Headers
            var newHeaders = new Dictionary<string, WireMockList<string>>();
            foreach (var header in original.Headers)
            {
                var templateHeaderKey = Template.Parse(header.Key);

                var templateHeaderValues = header.Value
                    .Select(Template.Parse)
                    .Select(x => x.Render(hash))
                    .ToArray();

                newHeaders.Add(templateHeaderKey.Render(hash), new WireMockList<string>(templateHeaderValues));
            }

            responseMessage.Headers = newHeaders;

            switch (original.StatusCode)
            {
                case int statusCodeAsInteger:
                    responseMessage.StatusCode = statusCodeAsInteger;
                    break;

                case string statusCodeAsString:
                    responseMessage.StatusCode = Template.Parse(statusCodeAsString).Render(hash);
                    break;
            }

            return responseMessage;
        }

        private static void TransformBodyAsJson(Hash hash, ResponseMessage original, ResponseMessage responseMessage)
        {
            JToken jToken;
            switch (original.BodyData.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jToken = bodyAsJObject.DeepClone();
                    WalkNode(jToken, hash);
                    break;

                case Array bodyAsArray:
                    jToken = JArray.FromObject(bodyAsArray);
                    WalkNode(jToken, hash);
                    break;

                case string bodyAsString:
                    jToken = ReplaceSingleNode(bodyAsString, hash);
                    break;

                default:
                    jToken = JObject.FromObject(original.BodyData.BodyAsJson);
                    WalkNode(jToken, hash);
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

        private static JToken ReplaceSingleNode(string stringValue, Hash context)
        {
            string transformedString = Template.Parse(stringValue).Render(context);
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

        private static void WalkNode(JToken node, Hash context)
        {
            if (node.Type == JTokenType.Object)
            {
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JProperty child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(child.Value, context);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JToken child in node.Children().ToArray())
                {
                    WalkNode(child, context);
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

                string transformedString = Template.Parse(stringValue).Render(context);
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

        private static void TransformBodyAsString(Hash template, ResponseMessage original, ResponseMessage responseMessage)
        {
            responseMessage.BodyData = new BodyData
            {
                Encoding = original.BodyData.Encoding,
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsString = Template.Parse(original.BodyData.BodyAsString).Render(template)
            };
        }

        private void TransformBodyAsFile(Hash hash, ResponseMessage original, ResponseMessage responseMessage, bool useTransformerForBodyAsFile)
        {
            string transformedBodyAsFilename = Template.Parse(original.BodyData.BodyAsFile).Render(hash);

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
                string text = _transformerContext.FileSystemHandler.ReadResponseBodyAsString(transformedBodyAsFilename);
                var templateBodyAsString = Template.Parse(text);

                responseMessage.BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                    BodyAsString = templateBodyAsString.Render(hash),
                    BodyAsFile = transformedBodyAsFilename
                };
            }
        }
    }
}
﻿using HandlebarsDotNet;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal class ResponseMessageTransformer
    {
        private readonly IHandlebarsContextFactory _factory;

        public ResponseMessageTransformer([NotNull] IHandlebarsContextFactory factory)
        {
            Check.NotNull(factory, nameof(factory));

            _factory = factory;
        }

        public ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile)
        {
            var handlebarsContext = _factory.Create();

            var responseMessage = new ResponseMessage();

            var template = new { request = requestMessage };

            switch (original.BodyData?.DetectedBodyType)
            {
                case BodyType.Json:
                    TransformBodyAsJson(handlebarsContext.Handlebars, template, original, responseMessage);
                    break;

                case BodyType.File:
                    TransformBodyAsFile(handlebarsContext, template, original, responseMessage, useTransformerForBodyAsFile);
                    break;

                case BodyType.String:
                    responseMessage.BodyOriginal = original.BodyData.BodyAsString;
                    TransformBodyAsString(handlebarsContext.Handlebars, template, original, responseMessage);
                    break;
            }

            responseMessage.FaultType = original.FaultType;
            responseMessage.FaultPercentage = original.FaultPercentage;

            // Headers
            var newHeaders = new Dictionary<string, WireMockList<string>>();
            foreach (var header in original.Headers)
            {
                var templateHeaderKey = handlebarsContext.Handlebars.Compile(header.Key);
                var templateHeaderValues = header.Value
                    .Select(handlebarsContext.Handlebars.Compile)
                    .Select(func => func(template))
                    .ToArray();

                newHeaders.Add(templateHeaderKey(template), new WireMockList<string>(templateHeaderValues));
            }

            responseMessage.Headers = newHeaders;

            switch (original.StatusCode)
            {
                case int statusCodeAsInteger:
                    responseMessage.StatusCode = statusCodeAsInteger;
                    break;

                case string statusCodeAsString:
                    var templateForStatusCode = handlebarsContext.Handlebars.Compile(statusCodeAsString);
                    responseMessage.StatusCode = templateForStatusCode(template);
                    break;
            }

            return responseMessage;
        }

        private static void TransformBodyAsJson(IHandlebars handlebarsContext, object template, ResponseMessage original, ResponseMessage responseMessage)
        {
            JToken jToken;
            switch (original.BodyData.BodyAsJson)
            {
                case JObject bodyAsJObject:
                    jToken = bodyAsJObject.DeepClone();
                    WalkNode(handlebarsContext, jToken, template);
                    break;

                case Array bodyAsArray:
                    jToken = JArray.FromObject(bodyAsArray);
                    WalkNode(handlebarsContext, jToken, template);
                    break;

                case string bodyAsString:
                    jToken = ReplaceSingleNode(handlebarsContext, bodyAsString, template);
                    break;

                default:
                    jToken = JObject.FromObject(original.BodyData.BodyAsJson);
                    WalkNode(handlebarsContext, jToken, template);
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

        private static JToken ReplaceSingleNode(IHandlebars handlebarsContext, string stringValue, object context)
        {
            var templateForStringValue = handlebarsContext.Compile(stringValue);
            string transformedString = templateForStringValue(context);
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

        private static void WalkNode(IHandlebars handlebarsContext, JToken node, object context)
        {
            if (node.Type == JTokenType.Object)
            {
                // In case of Object, loop all children. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JProperty child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(handlebarsContext, child.Value, context);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                // In case of Array, loop all items. Do a ToArray() to avoid `Collection was modified` exceptions.
                foreach (JToken child in node.Children().ToArray())
                {
                    WalkNode(handlebarsContext, child, context);
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

                var templateForStringValue = handlebarsContext.Compile(stringValue);
                string transformedString = templateForStringValue(context);
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

        private static void TransformBodyAsString(IHandlebars handlebarsContext, object template, ResponseMessage original, ResponseMessage responseMessage)
        {
            var templateBodyAsString = handlebarsContext.Compile(original.BodyData.BodyAsString);

            responseMessage.BodyData = new BodyData
            {
                Encoding = original.BodyData.Encoding,
                DetectedBodyType = original.BodyData.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                BodyAsString = templateBodyAsString(template)
            };
        }

        private void TransformBodyAsFile(IHandlebarsContext handlebarsContext, object template, ResponseMessage original, ResponseMessage responseMessage, bool useTransformerForBodyAsFile)
        {
            var templateBodyAsFile = handlebarsContext.Handlebars.Compile(original.BodyData.BodyAsFile);
            string transformedBodyAsFilename = templateBodyAsFile(template);

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
                var templateBodyAsString = handlebarsContext.Handlebars.Compile(text);

                responseMessage.BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = original.BodyData.DetectedBodyTypeFromContentType,
                    BodyAsString = templateBodyAsString(template),
                    BodyAsFile = transformedBodyAsFilename
                };
            }
        }
    }
}
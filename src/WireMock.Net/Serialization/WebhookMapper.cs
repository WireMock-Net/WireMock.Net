using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Http;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Serialization
{
    internal static class WebhookMapper
    {
        public static IWebhook Map(WebhookModel model)
        {
            var webhook = new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = model.Request.Url,
                    Method = model.Request.Method,
                    Headers = model.Request.Headers?.ToDictionary(x => x.Key, x => new WireMockList<string>(x.Value)) ?? new Dictionary<string, WireMockList<string>>()
                }
            };

            if (model.Request.UseTransformer == true)
            {
                webhook.Request.UseTransformer = true;
                if (!Enum.TryParse<TransformerType>(model.Request.TransformerType, out var transformerType))
                {
                    transformerType = TransformerType.Handlebars;
                }
                webhook.Request.TransformerType = transformerType;

                if (!Enum.TryParse<ReplaceNodeOption>(model.Request.TransformerReplaceNodeOption, out var option))
                {
                    option = ReplaceNodeOption.Bool;
                }

                webhook.Request.TransformerReplaceNodeOption = option;
            }

            IEnumerable<string> contentTypeHeader = null;
            if (webhook.Request.Headers.Any(header => string.Equals(header.Key, HttpKnownHeaderNames.ContentType, StringComparison.OrdinalIgnoreCase)))
            {
                contentTypeHeader = webhook.Request.Headers.First(header => string.Equals(header.Key, HttpKnownHeaderNames.ContentType, StringComparison.OrdinalIgnoreCase)).Value;
            }

            if (model.Request.Body != null)
            {
                webhook.Request.BodyData = new BodyData
                {
                    BodyAsString = model.Request.Body,
                    DetectedBodyType = BodyType.String,
                    DetectedBodyTypeFromContentType = BodyParser.DetectBodyTypeFromContentType(contentTypeHeader?.FirstOrDefault())
                };
            }
            else if (model.Request.BodyAsJson != null)
            {
                webhook.Request.BodyData = new BodyData
                {
                    BodyAsJson = model.Request.BodyAsJson,
                    DetectedBodyType = BodyType.Json,
                    DetectedBodyTypeFromContentType = BodyParser.DetectBodyTypeFromContentType(contentTypeHeader?.FirstOrDefault())
                };
            }

            return webhook;
        }

        public static WebhookModel Map(IWebhook webhook)
        {
            if (webhook?.Request == null)
            {
                return null;
            }

            var model = new WebhookModel
            {
                Request = new WebhookRequestModel
                {
                    Url = webhook.Request.Url,
                    Method = webhook.Request.Method,
                    Headers = webhook.Request.Headers?.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    UseTransformer = webhook.Request.UseTransformer,
                    TransformerType = webhook.Request.UseTransformer == true ? webhook.Request.TransformerType.ToString() : null,
                    TransformerReplaceNodeOption = webhook.Request.TransformerReplaceNodeOption.ToString()
                }
            };

            if (webhook.Request.BodyData != null)
            {
                switch (webhook.Request.BodyData.DetectedBodyType)
                {
                    case BodyType.String:
                        model.Request.Body = webhook.Request.BodyData.BodyAsString;
                        break;

                    case BodyType.Json:
                        model.Request.BodyAsJson = webhook.Request.BodyData.BodyAsJson;
                        break;

                    default:
                        // Empty
                        break;
                }
            }

            return model;
        }
    }
}
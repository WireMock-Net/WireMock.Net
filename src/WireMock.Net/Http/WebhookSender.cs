using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Models;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Transformers.Handlebars;
using WireMock.Transformers.Scriban;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Http
{
    internal class WebhookSender
    {
        private const string ClientIp = "::1";

        private readonly IWireMockServerSettings _settings;

        public WebhookSender(IWireMockServerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public Task<HttpResponseMessage> SendAsync([NotNull] HttpClient client, [NotNull] IWebhookRequest request, [NotNull] RequestMessage originalRequestMessage)
        {
            Check.NotNull(client, nameof(client));
            Check.NotNull(request, nameof(request));

            IBodyData bodyData;
            IDictionary<string, WireMockList<string>> headers;
            if (request.UseTransformer == true)
            {
                ITransformer responseMessageTransformer;
                switch (request.TransformerType)
                {
                    case TransformerType.Handlebars:
                        var factoryHandlebars = new HandlebarsContextFactory(_settings.FileSystemHandler, _settings.HandlebarsRegistrationCallback);
                        responseMessageTransformer = new Transformer(factoryHandlebars);
                        break;

                    case TransformerType.Scriban:
                    case TransformerType.ScribanDotLiquid:
                        var factoryDotLiquid = new ScribanContextFactory(_settings.FileSystemHandler, request.TransformerType);
                        responseMessageTransformer = new Transformer(factoryDotLiquid);
                        break;

                    default:
                        throw new NotImplementedException($"TransformerType '{request.TransformerType}' is not supported.");
                }

                var responseMessage = new ResponseMessage
                {
                    BodyData = request.BodyData,
                    Headers = request.Headers
                };

                var transformedResponseMessage = responseMessageTransformer.Transform(originalRequestMessage, responseMessage, false);
                bodyData = transformedResponseMessage.BodyData;
                headers = transformedResponseMessage.Headers;
            }
            else
            {
                bodyData = request.BodyData;
                headers = request.Headers;
            }

            // Create RequestMessage
            var requestMessage = new RequestMessage(
                new UrlDetails(request.Url),
                request.Method,
                ClientIp,
                bodyData,
                headers.ToDictionary(x => x.Key, x => x.Value.ToArray()))
            {
                DateTime = DateTime.UtcNow
            };

            // Create HttpRequestMessage
            var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, request.Url);

            // Call the URL
            return client.SendAsync(httpRequestMessage);
        }
    }
}
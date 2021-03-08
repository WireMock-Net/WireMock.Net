using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Models;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Validation;

namespace WireMock.Http
{
    internal class WebhookSender
    {
        private const string ClientIp = "::1";

        public Task<HttpResponseMessage> SendAsync([NotNull] HttpClient client, [NotNull] IWebhookRequest request)
        {
            Check.NotNull(client, nameof(client));
            Check.NotNull(request, nameof(request));

            // Create RequestMessage
            var requestMessage = new RequestMessage(
                new UrlDetails(request.Url),
                request.Method,
                ClientIp,
                request.BodyData,
                request.Headers.ToDictionary(x => x.Key, x => x.Value.ToArray()))
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Http
{
    internal static class HttpRequestMessageHelper
    {
        internal static HttpRequestMessage Create([NotNull] RequestMessage requestMessage, [NotNull] string url)
        {
            Check.NotNull(requestMessage, nameof(requestMessage));
            Check.NotNullOrEmpty(url, nameof(url));

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(requestMessage.Method), url);

            MediaTypeHeaderValue contentType = null;
            if (requestMessage.Headers != null && requestMessage.Headers.ContainsKey(HttpKnownHeaderNames.ContentType))
            {
                var value = requestMessage.Headers[HttpKnownHeaderNames.ContentType].FirstOrDefault();
                MediaTypeHeaderValue.TryParse(value, out contentType);
            }

            switch (requestMessage.BodyData?.DetectedBodyType)
            {
                case BodyType.Bytes:
                    httpRequestMessage.Content = new ByteArrayContent(requestMessage.BodyData.BodyAsBytes);
                    break;

                case BodyType.Json:
                    httpRequestMessage.Content = StringContentHelper.Create(JsonConvert.SerializeObject(requestMessage.BodyData.BodyAsJson), contentType);
                    break;

                case BodyType.String:
                    httpRequestMessage.Content = StringContentHelper.Create(requestMessage.BodyData.BodyAsString, contentType);
                    break;
            }

            // Overwrite the host header
            httpRequestMessage.Headers.Host = new Uri(url).Authority;

            // Set other headers if present
            if (requestMessage.Headers == null || requestMessage.Headers.Count == 0)
            {
                return httpRequestMessage;
            }

            var excludeHeaders = new List<string> { HttpKnownHeaderNames.Host, HttpKnownHeaderNames.ContentLength };
            if (contentType != null)
            {
                // Content-Type should be set on the content
                excludeHeaders.Add(HttpKnownHeaderNames.ContentType);
            }

            foreach (var header in requestMessage.Headers.Where(h => !excludeHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase)))
            {
                // Try to add to request headers. If failed - try to add to content headers
                if (httpRequestMessage.Headers.Contains(header.Key))
                {
                    continue;
                }

                if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    httpRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return httpRequestMessage;
        }
    }
}
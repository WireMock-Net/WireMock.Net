using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using MimeKit;
using Newtonsoft.Json;

namespace WireMock.Http
{
    internal static class HttpRequestMessageHelper
    {
        public static HttpRequestMessage Create(RequestMessage requestMessage, string url)
        {
            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(requestMessage.Method), url);

            ContentType contentType = null;
            if (requestMessage.Headers != null && requestMessage.Headers.ContainsKey(HttpKnownHeaderNames.ContentType))
            {
                var value = requestMessage.Headers[HttpKnownHeaderNames.ContentType].FirstOrDefault();
                ContentType.TryParse(value, out contentType);
            }

            // Set Body if present
            if (requestMessage.BodyAsBytes != null)
            {
                httpRequestMessage.Content = new ByteArrayContent(requestMessage.BodyAsBytes);
            }
            else if (requestMessage.BodyAsJson != null)
            {
                if (contentType != null)
                {
                    var encoding = requestMessage.BodyEncoding ?? Encoding.GetEncoding(contentType.Charset ?? "UTF-8");
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestMessage.BodyAsJson), encoding, contentType.MimeType);
                }
                else
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestMessage.BodyAsJson), requestMessage.BodyEncoding);
                }
            }
            else if (requestMessage.Body != null)
            {
                if (contentType != null)
                {
                    var encoding = requestMessage.BodyEncoding ?? Encoding.GetEncoding(contentType.Charset ?? "UTF-8");
                    httpRequestMessage.Content = new StringContent(requestMessage.Body, encoding, contentType.MimeType);
                }
                else
                {
                    httpRequestMessage.Content = new StringContent(requestMessage.Body, requestMessage.BodyEncoding);
                }
            }

            // Overwrite the host header
            httpRequestMessage.Headers.Host = new Uri(url).Authority;

            // Set other headers if present and if not excluded
            if (requestMessage.Headers == null || requestMessage.Headers.Count == 0)
            {
                return httpRequestMessage;
            }

            var excludeHeaders = new List<string> { HttpKnownHeaderNames.Host, HttpKnownHeaderNames.ContentLength };
            if (contentType != null)
            {
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
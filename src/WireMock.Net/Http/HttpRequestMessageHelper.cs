using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using MimeKit;
using Newtonsoft.Json;
using WireMock.Util;

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
            if (requestMessage.BodyData?.DetectedBodyType == BodyType.Bytes)
            {
                httpRequestMessage.Content = new ByteArrayContent(requestMessage.BodyData.BodyAsBytes);
            }
            else if (requestMessage.BodyData?.DetectedBodyType == BodyType.Json)
            {
                if (contentType != null)
                {
                    var encoding = requestMessage.BodyData.Encoding ?? Encoding.GetEncoding(contentType.Charset ?? "UTF-8");
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestMessage.BodyData.BodyAsJson), encoding, contentType.MimeType);
                }
                else
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestMessage.BodyData.BodyAsJson), requestMessage.BodyData.Encoding);
                }
            }
            else if (requestMessage.BodyData?.DetectedBodyType == BodyType.String)
            {
                if (contentType != null)
                {
                    var encoding = requestMessage.BodyData.Encoding ?? Encoding.GetEncoding(contentType.Charset ?? "UTF-8");
                    httpRequestMessage.Content = new StringContent(requestMessage.BodyData.BodyAsString, encoding, contentType.MimeType);
                }
                else
                {
                    httpRequestMessage.Content = new StringContent(requestMessage.BodyData.BodyAsString, requestMessage.BodyData.Encoding);
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
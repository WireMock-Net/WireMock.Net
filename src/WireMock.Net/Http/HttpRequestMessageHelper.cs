using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Types;

namespace WireMock.Http;

internal static class HttpRequestMessageHelper
{
    internal static HttpRequestMessage Create(IRequestMessage requestMessage, string url)
    {
        Guard.NotNull(requestMessage);
        Guard.NotNullOrEmpty(url);

        var httpRequestMessage = new HttpRequestMessage(new HttpMethod(requestMessage.Method), url);

        MediaTypeHeaderValue? contentType = null;
        if (requestMessage.Headers != null && requestMessage.Headers.ContainsKey(HttpKnownHeaderNames.ContentType))
        {
            var value = requestMessage.Headers[HttpKnownHeaderNames.ContentType].FirstOrDefault();
            MediaTypeHeaderValue.TryParse(value, out contentType);
        }

        switch (requestMessage.BodyData?.DetectedBodyType)
        {
            case BodyType.Bytes:
                httpRequestMessage.Content = ByteArrayContentHelper.Create(requestMessage.BodyData.BodyAsBytes!, contentType);
                break;

            case BodyType.Json:
                httpRequestMessage.Content = StringContentHelper.Create(JsonConvert.SerializeObject(requestMessage.BodyData.BodyAsJson), contentType);
                break;

            case BodyType.String:
            case BodyType.FormUrlEncoded:
                httpRequestMessage.Content = StringContentHelper.Create(requestMessage.BodyData.BodyAsString!, contentType);
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
            // Skip if already added. We need to ToList() else calling httpRequestMessage.Headers.Contains() with a header starting with a ":" throws an exception.
            if (httpRequestMessage.Headers.ToList().Any(h => string.Equals(h.Key, header.Key, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            // Skip if already added. We need to ToList() else calling httpRequestMessage.Content.Headers.Contains(...) with a header starting with a ":" throws an exception.
            if (httpRequestMessage.Content != null && httpRequestMessage.Content.Headers.ToList().Any(h => string.Equals(h.Key, header.Key, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            // Try to add to request headers. If failed - try to add to content headers. If still fails, just ignore this header.
            try
            {
                if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    httpRequestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            catch
            {
                // Just continue
            }
        }

        return httpRequestMessage;
    }
}
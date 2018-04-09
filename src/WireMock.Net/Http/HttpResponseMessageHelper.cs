using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Util;

namespace WireMock.Http
{
    internal static class HttpResponseMessageHelper
    {
        public static async Task<ResponseMessage> Create(HttpResponseMessage httpResponseMessage, Uri requiredUri, Uri originalUri)
        {
            var responseMessage = new ResponseMessage { StatusCode = (int)httpResponseMessage.StatusCode };

            // Set both content and response headers, replacing URLs in values
            var headers = (httpResponseMessage.Content?.Headers.Union(httpResponseMessage.Headers) ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>()).ToArray();
            if (httpResponseMessage.Content != null)
            {
                var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                IEnumerable<string> contentTypeHeader = null;
                if (headers.Any(header => string.Equals(header.Key, HttpKnownHeaderNames.ContentType, StringComparison.OrdinalIgnoreCase)))
                {
                    contentTypeHeader = headers.First(header => string.Equals(header.Key, HttpKnownHeaderNames.ContentType, StringComparison.OrdinalIgnoreCase)).Value;
                }

                var body = await BodyParser.Parse(stream, contentTypeHeader?.FirstOrDefault());
                responseMessage.Body = body.BodyAsString;
                responseMessage.BodyAsJson = body.BodyAsJson;
                responseMessage.BodyAsBytes = body.BodyAsBytes;
            }

            foreach (var header in headers)
            {
                // If Location header contains absolute redirect URL, and base URL is one that we proxy to,
                // we need to replace it to original one.
                if (string.Equals(header.Key, HttpKnownHeaderNames.Location, StringComparison.OrdinalIgnoreCase)
                    && Uri.TryCreate(header.Value.First(), UriKind.Absolute, out Uri absoluteLocationUri)
                    && string.Equals(absoluteLocationUri.Host, requiredUri.Host, StringComparison.OrdinalIgnoreCase))
                {
                    var replacedLocationUri = new Uri(originalUri, absoluteLocationUri.PathAndQuery);
                    responseMessage.AddHeader(header.Key, replacedLocationUri.ToString());
                }
                else
                {
                    responseMessage.AddHeader(header.Key, header.Value.ToArray());
                }
            }

            return responseMessage;
        }
    }
}
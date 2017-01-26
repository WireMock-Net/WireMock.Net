using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace WireMock
{
    /// <summary>
    /// The http listener request mapper.
    /// </summary>
    public class HttpListenerRequestMapper
    {
        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="listenerRequest">The listener request.</param>
        /// <returns>The <see cref="RequestMessage"/>.</returns>
        public RequestMessage Map(HttpListenerRequest listenerRequest)
        {
            Uri url = listenerRequest.Url;
            string verb = listenerRequest.HttpMethod;
            byte[] body = GetRequestBody(listenerRequest);
            string bodyAsString = body != null ? listenerRequest.ContentEncoding.GetString(body) : null;
            var listenerHeaders = listenerRequest.Headers;
            var headers = listenerHeaders.AllKeys.ToDictionary(k => k, k => listenerHeaders[k]);
            var cookies = new Dictionary<string, string>();
            foreach (Cookie cookie in listenerRequest.Cookies)
                cookies.Add(cookie.Name, cookie.Value);

            var message = new RequestMessage(url, verb, body, bodyAsString, headers, cookies) { DateTime = DateTime.Now };

            return message;
        }

        /// <summary>
        /// The get request body.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private byte[] GetRequestBody(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var bodyStream = request.InputStream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    bodyStream.CopyTo(memoryStream);

                    return memoryStream.ToArray();
                }
            }
        }
    }
}
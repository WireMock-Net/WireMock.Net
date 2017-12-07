using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.Util;
#if !NETSTANDARD
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Owin
{
    /// <summary>
    /// OwinResponseMapper
    /// </summary>
    public class OwinResponseMapper
    {
        private readonly Encoding _utf8NoBom = new UTF8Encoding(false);

        // https://stackoverflow.com/questions/239725/cannot-set-some-http-headers-when-using-system-net-webrequest
#if !NETSTANDARD
        private static readonly IDictionary<string, Action<IOwinResponse, WireMockList<string>>> RestrictedResponseHeaders = new Dictionary<string, Action<IOwinResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#else
        private static readonly IDictionary<string, Action<HttpResponse, WireMockList<string>>> RestrictedResponseHeaders = new Dictionary<string, Action<HttpResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#endif
            { "Content-Length", null },
            { "Content-Type", (r, v) => r.ContentType = v.FirstOrDefault() },
            { "Keep-Alive", null },
            { "Transfer-Encoding", null },
            { "WWW-Authenticate", null }
        };

        /// <summary>
        /// MapAsync ResponseMessage to OwinResponse
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="response"></param>
        public async Task MapAsync(ResponseMessage responseMessage
#if !NETSTANDARD
            , IOwinResponse response
#else
            , HttpResponse response
#endif
            )
        {
            response.StatusCode = responseMessage.StatusCode;

            // Set headers
            foreach (var pair in responseMessage.Headers)
            {
                if (RestrictedResponseHeaders.ContainsKey(pair.Key))
                {
                    RestrictedResponseHeaders[pair.Key]?.Invoke(response, pair.Value);
                }
                else
                {
#if !NETSTANDARD
                    response.Headers.AppendValues(pair.Key, pair.Value.ToArray());
#else
                    response.Headers.Append(pair.Key, pair.Value.ToArray());
#endif
                }
            }

            if (responseMessage.Body == null && responseMessage.BodyAsBytes == null && responseMessage.BodyAsFile == null)
            {
                return;
            }

            if (responseMessage.BodyAsBytes != null)
            {
                await response.Body.WriteAsync(responseMessage.BodyAsBytes, 0, responseMessage.BodyAsBytes.Length);
                return;
            }

            if (responseMessage.BodyAsFile != null)
            {
                byte[] bytes = File.ReadAllBytes(responseMessage.BodyAsFile);

                await response.Body.WriteAsync(bytes, 0, bytes.Length);
                return;
            }

            Encoding encoding = responseMessage.BodyEncoding ?? _utf8NoBom;
            using (var writer = new StreamWriter(response.Body, encoding))
            {
                await writer.WriteAsync(responseMessage.Body);
                // TODO : response.ContentLength = responseMessage.Body.Length;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.Http;
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

        // https://msdn.microsoft.com/en-us/library/78h415ay(v=vs.110).aspx
#if !NETSTANDARD
        private static readonly IDictionary<string, Action<IOwinResponse, WireMockList<string>>> RestrictedResponseHeaders = new Dictionary<string, Action<IOwinResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#else
        private static readonly IDictionary<string, Action<HttpResponse, WireMockList<string>>> RestrictedResponseHeaders = new Dictionary<string, Action<HttpResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#endif
            { HttpKnownHeaderNames.Accept, null },
            { HttpKnownHeaderNames.Connection, null },
            { HttpKnownHeaderNames.ContentLength, null },
            { HttpKnownHeaderNames.ContentType, (r, v) => r.ContentType = v.FirstOrDefault() },
            { HttpKnownHeaderNames.Date, null },
            { HttpKnownHeaderNames.Expect, null },
            { HttpKnownHeaderNames.Host, null },
            { HttpKnownHeaderNames.IfModifiedSince, null },
            { HttpKnownHeaderNames.KeepAlive, null },
            { HttpKnownHeaderNames.Range, null },
            { HttpKnownHeaderNames.Referer, null },
            { HttpKnownHeaderNames.TransferEncoding, null },
            { HttpKnownHeaderNames.UserAgent, null },
            { HttpKnownHeaderNames.ProxyConnection, null },
            { HttpKnownHeaderNames.WWWAuthenticate, null }
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

            if (responseMessage.Body == null && responseMessage.BodyAsBytes == null && responseMessage.BodyAsFile == null && responseMessage.BodyAsJson == null)
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

            if (responseMessage.BodyAsJson != null)
            {
                Formatting formatting = responseMessage.BodyAsJsonIndented == true ? Formatting.Indented : Formatting.None;
                string jsonBody = JsonConvert.SerializeObject(responseMessage.BodyAsJson, new JsonSerializerSettings { Formatting = formatting, NullValueHandling = NullValueHandling.Ignore });
                using (var writer = new StreamWriter(response.Body, responseMessage.BodyEncoding ?? _utf8NoBom))
                {
                    await writer.WriteAsync(jsonBody);
                }

                return;
            }

            using (var writer = new StreamWriter(response.Body, responseMessage.BodyEncoding ?? _utf8NoBom))
            {
                await writer.WriteAsync(responseMessage.Body);
            }
        }
    }
}
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
        private static readonly IDictionary<string, Action<IOwinResponse, WireMockList<string>>> ResponseHeadersToFix = new Dictionary<string, Action<IOwinResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#else
        private static readonly IDictionary<string, Action<HttpResponse, WireMockList<string>>> ResponseHeadersToFix = new Dictionary<string, Action<HttpResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#endif
            //{ HttpKnownHeaderNames.Accept, null },
            //{ HttpKnownHeaderNames.Connection, null },
            //{ HttpKnownHeaderNames.ContentLength, null },
            { HttpKnownHeaderNames.ContentType, (r, v) => r.ContentType = v.FirstOrDefault() }
            //{ HttpKnownHeaderNames.Date, null },
            //{ HttpKnownHeaderNames.Expect, null },
            //{ HttpKnownHeaderNames.Host, null },
            //{ HttpKnownHeaderNames.IfModifiedSince, null },
            //{ HttpKnownHeaderNames.KeepAlive, null },
            //{ HttpKnownHeaderNames.Range, null },
            //{ HttpKnownHeaderNames.Referer, null },
            //{ HttpKnownHeaderNames.TransferEncoding, null },
            //{ HttpKnownHeaderNames.UserAgent, null },
            //{ HttpKnownHeaderNames.ProxyConnection, null },
            //{ HttpKnownHeaderNames.WWWAuthenticate, null }
        };

        private void SetResponseHeaders(ResponseMessage responseMessage
#if !NETSTANDARD
            , IOwinResponse response
#else
            , HttpResponse response
#endif
        )
        {
            // Set headers
            foreach (var pair in responseMessage.Headers)
            {
                if (ResponseHeadersToFix.ContainsKey(pair.Key))
                {
                    ResponseHeadersToFix[pair.Key]?.Invoke(response, pair.Value);
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
        }

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

            if (responseMessage.Body == null && responseMessage.BodyAsBytes == null && responseMessage.BodyAsFile == null && responseMessage.BodyAsJson == null)
            {
                SetResponseHeaders(responseMessage, response);
                return;
            }

            if (responseMessage.BodyAsBytes != null)
            {
                await response.Body.WriteAsync(responseMessage.BodyAsBytes, 0, responseMessage.BodyAsBytes.Length);

                SetResponseHeaders(responseMessage, response);
                return;
            }

            if (responseMessage.BodyAsFile != null)
            {
                byte[] bytes = File.ReadAllBytes(responseMessage.BodyAsFile);

                await response.Body.WriteAsync(bytes, 0, bytes.Length);

                SetResponseHeaders(responseMessage, response);
                return;
            }

            if (responseMessage.BodyAsJson != null)
            {
                Formatting formatting = responseMessage.BodyAsJsonIndented == true ? Formatting.Indented : Formatting.None;
                string jsonBody = JsonConvert.SerializeObject(responseMessage.BodyAsJson, new JsonSerializerSettings { Formatting = formatting, NullValueHandling = NullValueHandling.Ignore });
                using (var writer = new StreamWriter(response.Body, responseMessage.BodyEncoding ?? _utf8NoBom))
                {
                    await writer.WriteAsync(jsonBody);

                    SetResponseHeaders(responseMessage, response);
                }

                return;
            }

            using (var writer = new StreamWriter(response.Body, responseMessage.BodyEncoding ?? _utf8NoBom))
            {
                await writer.WriteAsync(responseMessage.Body);

                SetResponseHeaders(responseMessage, response);
            }
        }
    }
}
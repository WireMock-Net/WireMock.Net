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
            { HttpKnownHeaderNames.ContentType, (r, v) => r.ContentType = v.FirstOrDefault() }
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
        /// Map ResponseMessage to OwinResponse/HttpResponse
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

            byte[] bytes = null;
            if (responseMessage.BodyAsBytes != null)
            {
                bytes = responseMessage.BodyAsBytes;
            }
            else if (responseMessage.BodyAsFile != null)
            {
                bytes = File.ReadAllBytes(responseMessage.BodyAsFile);
            }
            else if (responseMessage.BodyAsJson != null)
            {
                Formatting formatting = responseMessage.BodyAsJsonIndented == true ? Formatting.Indented : Formatting.None;
                string jsonBody = JsonConvert.SerializeObject(responseMessage.BodyAsJson, new JsonSerializerSettings { Formatting = formatting, NullValueHandling = NullValueHandling.Ignore });
                bytes = (responseMessage.BodyEncoding ?? _utf8NoBom).GetBytes(jsonBody);
            }
            else if (responseMessage.Body != null)
            {
                bytes = (responseMessage.BodyEncoding ?? _utf8NoBom).GetBytes(responseMessage.Body);
            }

            SetResponseHeaders(responseMessage, response);

            if (bytes != null)
            {
                await response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.Http;
using WireMock.Util;
#if !USE_ASPNETCORE
using System.Net;
using Microsoft.Owin;
using IResponse = Microsoft.Owin.IOwinResponse;
#else
using Microsoft.AspNetCore.Http;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace WireMock.Owin.Mappers
{
    /// <summary>
    /// OwinResponseMapper
    /// </summary>
    public class OwinResponseMapper : IOwinResponseMapper
    {
        private readonly Encoding _utf8NoBom = new UTF8Encoding(false);

        // https://msdn.microsoft.com/en-us/library/78h415ay(v=vs.110).aspx
#if !USE_ASPNETCORE
        private static readonly IDictionary<string, Action<IResponse, WireMockList<string>>> ResponseHeadersToFix = new Dictionary<string, Action<IResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#else
        private static readonly IDictionary<string, Action<IResponse, WireMockList<string>>> ResponseHeadersToFix = new Dictionary<string, Action<IResponse, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase) {
#endif
            { HttpKnownHeaderNames.ContentType, (r, v) => r.ContentType = v.FirstOrDefault() }
        };

        /// <inheritdoc cref="IOwinResponseMapper.MapAsync"/>
        public async Task MapAsync(ResponseMessage responseMessage, IResponse response)
        {
            if (responseMessage == null)
            {
                return;
            }

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

        private void SetResponseHeaders(ResponseMessage responseMessage, IResponse response)
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
#if !USE_ASPNETCORE
                    // For non-NETSTANDARD, check if this response header can be added (#148)
                    if (!WebHeaderCollection.IsRestricted(pair.Key, true))
                    {
                        response.Headers.AppendValues(pair.Key, pair.Value.ToArray());
                    }
#else
                    // NETSTANDARD can add any header (or so it seems)
                    response.Headers.Append(pair.Key, pair.Value.ToArray());
#endif
                }
            }
        }
    }
}
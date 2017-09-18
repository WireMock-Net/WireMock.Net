using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !NETSTANDARD
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
#endif

namespace WireMock.Owin
{
    /// <summary>
    /// OwinRequestMapper
    /// </summary>
    public class OwinRequestMapper
    {
        /// <summary>
        /// MapAsync IOwinRequest to RequestMessage
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RequestMessage> MapAsync(
#if !NETSTANDARD
            IOwinRequest request
#else
            HttpRequest request
#endif
            )
        {
#if !NETSTANDARD
            Uri url = request.Uri;
            string clientIP = request.RemoteIpAddress;
#else
            Uri url = new Uri(request.GetEncodedUrl());
            var connection = request.HttpContext.Connection;
            string clientIP = connection.RemoteIpAddress.IsIPv4MappedToIPv6
                ? connection.RemoteIpAddress.MapToIPv4().ToString()
                : connection.RemoteIpAddress.ToString();
#endif
            string method = request.Method;

            string bodyAsString = null;
            byte[] body = null;
            Encoding bodyEncoding = null;
            if (request.Body != null)
            {
                using (var streamReader = new StreamReader(request.Body))
                {
                    bodyAsString = await streamReader.ReadToEndAsync();
                    bodyEncoding = streamReader.CurrentEncoding;
                }

                body = bodyEncoding.GetBytes(bodyAsString);
            }

            var listenerHeaders = request.Headers;

            var headers = new Dictionary<string, string>();
            foreach (var header in listenerHeaders)
                headers.Add(header.Key, header.Value.FirstOrDefault());

            var cookies = new Dictionary<string, string>();

            foreach (var cookie in request.Cookies)
                cookies.Add(cookie.Key, cookie.Value);

            return new RequestMessage(url, method, clientIP, body, bodyAsString, bodyEncoding, headers, cookies) { DateTime = DateTime.Now };
        }
    }
}
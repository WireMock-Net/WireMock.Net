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
            if (ParseBody(method) && request.Body != null)
            {
                using (var streamReader = new StreamReader(request.Body))
                {
                    bodyAsString = await streamReader.ReadToEndAsync();
                    bodyEncoding = streamReader.CurrentEncoding;
                }

                body = bodyEncoding.GetBytes(bodyAsString);
            }

            Dictionary<string, string[]> headers = null;
            if (request.Headers.Any())
            {
                headers = new Dictionary<string, string[]>();
                foreach (var header in request.Headers)
                {
                    headers.Add(header.Key, header.Value);
                }
            }

            IDictionary<string, string> cookies = null;
            if (request.Cookies.Any())
            {
                cookies = new Dictionary<string, string>();
                foreach (var cookie in request.Cookies)
                {
                    cookies.Add(cookie.Key, cookie.Value);
                }
            }

            return new RequestMessage(url, method, clientIP, body, bodyAsString, bodyEncoding, headers, cookies) { DateTime = DateTime.Now };
        }

        private bool ParseBody(string method)
        {
            /*
                HEAD - No defined body semantics.
                GET - No defined body semantics.
                PUT - Body supported.
                POST - Body supported.
                DELETE - No defined body semantics.
                TRACE - Body not supported.
                OPTIONS - Body supported but no semantics on usage (maybe in the future).
                CONNECT - No defined body semantics
                PATCH - Body supported.
            */
            return new[] { "PUT", "POST", "OPTIONS", "PATCH" }.Contains(method.ToUpper());
        }
    }
}
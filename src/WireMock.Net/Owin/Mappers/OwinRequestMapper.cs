using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WireMock.Http;
using WireMock.Models;
using WireMock.Util;
#if !USE_ASPNETCORE
using IRequest = Microsoft.Owin.IOwinRequest;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using IRequest = Microsoft.AspNetCore.Http.HttpRequest;
#endif

namespace WireMock.Owin.Mappers
{
    /// <summary>
    /// OwinRequestMapper
    /// </summary>
    internal class OwinRequestMapper : IOwinRequestMapper
    {
        /// <inheritdoc cref="IOwinRequestMapper.MapAsync"/>
        public async Task<RequestMessage> MapAsync(IRequest request, IWireMockMiddlewareOptions options)
        {
            (UrlDetails urldetails, string clientIP) = ParseRequest(request);

            string method = request.Method;

            Dictionary<string, string[]> headers = null;
            IEnumerable<string> contentEncodingHeader = null;
            if (request.Headers.Any())
            {
                headers = new Dictionary<string, string[]>();
                foreach (var header in request.Headers)
                {
                    headers.Add(header.Key, header.Value);

                    if (string.Equals(header.Key, HttpKnownHeaderNames.ContentEncoding, StringComparison.OrdinalIgnoreCase))
                    {
                        contentEncodingHeader = header.Value;
                    }
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

            BodyData body = null;
            if (request.Body != null && BodyParser.ShouldParseBody(method, options.AllowBodyForAllHttpMethods == true))
            {
                var bodyParserSettings = new BodyParserSettings
                {
                    Stream = request.Body,
                    ContentType = request.ContentType,
                    DeserializeJson = !options.DisableJsonBodyParsing.GetValueOrDefault(false),
                    ContentEncoding = contentEncodingHeader?.FirstOrDefault(),
                    DecompressGZipAndDeflate = !options.DisableRequestBodyDecompressing.GetValueOrDefault(false)
                };

                body = await BodyParser.Parse(bodyParserSettings);
            }

            return new RequestMessage(urldetails, method, clientIP, body, headers, cookies) { DateTime = DateTime.UtcNow };
        }

        private (UrlDetails UrlDetails, string ClientIP) ParseRequest(IRequest request)
        {
#if !USE_ASPNETCORE
            var urldetails = UrlUtils.Parse(request.Uri, request.PathBase);
            string clientIP = request.RemoteIpAddress;
#else
            var urldetails = UrlUtils.Parse(new Uri(request.GetEncodedUrl()), request.PathBase);
            var connection = request.HttpContext.Connection;
            string clientIP = connection.RemoteIpAddress.IsIPv4MappedToIPv6
                ? connection.RemoteIpAddress.MapToIPv4().ToString()
                : connection.RemoteIpAddress.ToString();
#endif
            return (urldetails, clientIP);
        }
    }
}
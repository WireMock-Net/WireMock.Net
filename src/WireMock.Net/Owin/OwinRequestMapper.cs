using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

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
        public async Task<RequestMessage> MapAsync(IOwinRequest request)
        {
            Uri url = request.Uri;
            string verb = request.Method;

            string bodyAsString = null;
            byte[] body = null;
            Encoding bodyEncoding = null;
            if (request.Body != null && request.Body.Length > 0)
            {
                using (var sr = new StreamReader(request.Body))
                {
                    bodyEncoding = sr.CurrentEncoding;
                    bodyAsString = await sr.ReadToEndAsync();
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

            return new RequestMessage(url, verb, body, bodyAsString, bodyEncoding, headers, cookies) { DateTime = DateTime.Now };
        }
    }
}
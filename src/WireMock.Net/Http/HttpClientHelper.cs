using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WireMock.Http
{
    internal static class HttpClientHelper
    {
        private static HttpClient CreateHttpClient(string clientX509Certificate2Filename = null)
        {
            if (!string.IsNullOrEmpty(clientX509Certificate2Filename))
            {
#if NETSTANDARD || NET46
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                handler.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(clientX509Certificate2Filename));
                return new HttpClient(handler);
#else
                var handler = new WebRequestHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
                };

                handler.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(clientX509Certificate2Filename));
                return new HttpClient(handler);
#endif
            }

            return new HttpClient();
        }

        public static async Task<ResponseMessage> SendAsync(RequestMessage requestMessage, string url, string clientX509Certificate2Filename = null)
        {
            var client = CreateHttpClient(clientX509Certificate2Filename);

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(requestMessage.Method), url);

            // Overwrite the host header
            httpRequestMessage.Headers.Host = new Uri(url).Authority;

            // Set headers if present
            if (requestMessage.Headers != null)
            {
                foreach (var headerName in requestMessage.Headers.Keys.Where(k => k.ToUpper() != "HOST"))
                {
                    httpRequestMessage.Headers.Add(headerName, new[] { requestMessage.Headers[headerName] });
                }
            }

            // Set Body if present
            if (requestMessage.BodyAsBytes != null && requestMessage.BodyAsBytes.Length > 0)
            {
                httpRequestMessage.Content = new ByteArrayContent(requestMessage.BodyAsBytes);
            }

            // Call the URL
            var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead);

            // Transform response
            var responseMessage = new ResponseMessage
            {
                StatusCode = (int)httpResponseMessage.StatusCode,
                Body = await httpResponseMessage.Content.ReadAsStringAsync()
            };

            foreach (var header in httpResponseMessage.Headers)
            {
                responseMessage.AddHeader(header.Key, header.Value.FirstOrDefault());
            }

            return responseMessage;
        }
    }
}
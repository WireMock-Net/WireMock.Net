using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace WireMock.Http
{
    internal static class HttpClientHelper
    {

        private static HttpClient CreateHttpClient(string clientX509Certificate2ThumbprintOrSubjectName = null)
        {
            if (!string.IsNullOrEmpty(clientX509Certificate2ThumbprintOrSubjectName))
            {
#if NETSTANDARD || NET46
                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                var x509Certificate2 = CertificateUtil.GetCertificate(clientX509Certificate2ThumbprintOrSubjectName);
                handler.ClientCertificates.Add(x509Certificate2);


#else
                var handler = new WebRequestHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };

                var x509Certificate2 = CertificateUtil.GetCertificate(clientX509Certificate2ThumbprintOrSubjectName);
                handler.ClientCertificates.Add(x509Certificate2);
                return new HttpClient(handler);
#endif
            }

            return new HttpClient();
        }

        public static async Task<ResponseMessage> SendAsync(RequestMessage requestMessage, string url, string clientX509Certificate2ThumbprintOrSubjectName = null)
        {
            var client = CreateHttpClient(clientX509Certificate2ThumbprintOrSubjectName);

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

                // TODO : what about BodyAsBytes ???
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
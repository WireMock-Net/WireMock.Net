using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Validation;

namespace WireMock.Http
{
    internal static class HttpClientHelper
    {
        public static HttpClient CreateHttpClient(string clientX509Certificate2ThumbprintOrSubjectName = null)
        {
            HttpClientHandler handler;

            if (string.IsNullOrEmpty(clientX509Certificate2ThumbprintOrSubjectName))
            {
                handler = new HttpClientHandler();
            }
            else
            {
#if NETSTANDARD || NET46
                handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                var x509Certificate2 = CertificateUtil.GetCertificate(clientX509Certificate2ThumbprintOrSubjectName);
                handler.ClientCertificates.Add(x509Certificate2);
#else

                var webRequestHandler = new WebRequestHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true,
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                };

                var x509Certificate2 = CertificateUtil.GetCertificate(clientX509Certificate2ThumbprintOrSubjectName);
                webRequestHandler.ClientCertificates.Add(x509Certificate2);
                handler = webRequestHandler;
#endif
            }

            // For proxy we shouldn't follow auto redirects
            handler.AllowAutoRedirect = false;

            // If UseCookies enabled, httpClient ignores Cookie header
            handler.UseCookies = false;

            return new HttpClient(handler);
        }

        public static async Task<ResponseMessage> SendAsync(HttpClient client, RequestMessage requestMessage, string url)
        {
            Check.NotNull(client, nameof(client));

            var originalUri = new Uri(requestMessage.Url);
            var requiredUri = new Uri(url);

            var httpRequestMessage = new HttpRequestMessage(new HttpMethod(requestMessage.Method), url);

            // Set Body if present
            if (requestMessage.BodyAsBytes != null && requestMessage.BodyAsBytes.Length > 0)
            {
                httpRequestMessage.Content = new ByteArrayContent(requestMessage.BodyAsBytes);
            }

            // Overwrite the host header
            httpRequestMessage.Headers.Host = requiredUri.Authority;

            // Set headers if present
            if (requestMessage.Headers != null)
            {
                foreach (var header in requestMessage.Headers.Where(header => !string.Equals(header.Key, "HOST", StringComparison.OrdinalIgnoreCase)))
                {
                    // Try to add to request headers. If failed - try to add to content headers
                    if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        httpRequestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            // Call the URL
            var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead);

            // Transform response
            var responseMessage = new ResponseMessage
            {
                StatusCode = (int)httpResponseMessage.StatusCode,

                BodyAsBytes = await httpResponseMessage.Content.ReadAsByteArrayAsync(),
                Body = await httpResponseMessage.Content.ReadAsStringAsync()
            };

            // Set both content and response headers, replacing URLs in values
            var headers = httpResponseMessage.Content?.Headers.Union(httpResponseMessage.Headers);

            foreach (var header in headers)
            {
                // if Location header contains absolute redirect URL, and base URL is one that we proxy to,
                // we need to replace it to original one.
                if (string.Equals(header.Key, "Location", StringComparison.OrdinalIgnoreCase)
                    && Uri.TryCreate(header.Value.First(), UriKind.Absolute, out Uri absoluteLocationUri)
                    && string.Equals(absoluteLocationUri.Host, requiredUri.Host, StringComparison.OrdinalIgnoreCase))
                {
                    var replacedLocationUri = new Uri(originalUri, absoluteLocationUri.PathAndQuery);
                    responseMessage.AddHeader(header.Key, replacedLocationUri.ToString());
                }
                else
                {
                    responseMessage.AddHeader(header.Key, header.Value.ToArray());
                }
            }

            return responseMessage;
        }
    }
}
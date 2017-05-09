using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WireMock.Http
{
    internal static class HttpClientHelper
    {
        public static async Task<ResponseMessage> SendAsync(RequestMessage requestMessage, string url)
        {
            using (var client = new HttpClient())
            {
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
}
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.Http;
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

        /// <summary>
        /// MapAsync ResponseMessage to OwinResponse
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

            if (responseMessage.Headers.ContainsKey(HttpKnownHeaderNames.ContentType))
            {
                response.ContentType = responseMessage.Headers[HttpKnownHeaderNames.ContentType][0];
            }
            var headers = responseMessage.Headers.Where(h => h.Key != HttpKnownHeaderNames.ContentType).ToList();

#if !NETSTANDARD
            headers.ForEach(pair => response.Headers.AppendValues(pair.Key, pair.Value));
#else
            headers.ForEach(pair => response.Headers.Append(pair.Key, pair.Value));
#endif

            if (responseMessage.Body == null && responseMessage.BodyAsBytes == null && responseMessage.BodyAsFile == null)
            {
                return;
            }

            if (responseMessage.BodyAsBytes != null)
            {
                await response.Body.WriteAsync(responseMessage.BodyAsBytes, 0, responseMessage.BodyAsBytes.Length);
                return;
            }

            if (responseMessage.BodyAsFile != null)
            {
                byte[] bytes = File.ReadAllBytes(responseMessage.BodyAsFile);

                await response.Body.WriteAsync(bytes, 0, bytes.Length);
                return;
            }

            Encoding encoding = responseMessage.BodyEncoding ?? _utf8NoBom;
            using (var writer = new StreamWriter(response.Body, encoding))
            {
                await writer.WriteAsync(responseMessage.Body);
                // TODO : response.ContentLength = responseMessage.Body.Length;
            }
        }
    }
}
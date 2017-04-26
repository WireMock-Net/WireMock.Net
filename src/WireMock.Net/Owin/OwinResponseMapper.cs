using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET45
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
#if NET45
            , IOwinResponse response
#else
            , HttpResponse response
#endif
            )
        {
            response.StatusCode = responseMessage.StatusCode;

            responseMessage.Headers.ToList().ForEach(pair => response.Headers.Append(pair.Key, pair.Value));

            if (responseMessage.Body == null)
                return;

            Encoding encoding = responseMessage.BodyEncoding ?? _utf8NoBom;
            using (var writer = new StreamWriter(response.Body, encoding))
            {
                await writer.WriteAsync(responseMessage.Body);
            }
        }
    }
}
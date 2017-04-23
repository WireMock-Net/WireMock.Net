using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

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
        public async Task MapAsync(ResponseMessage responseMessage, IOwinResponse response)
        {
            response.StatusCode = responseMessage.StatusCode;

            WriteHeaders(responseMessage, response);

            await WriteBodyAsync(responseMessage, response);
        }

        private void WriteHeaders(ResponseMessage responseMessage, IOwinResponse response)
        {
            responseMessage.Headers.ToList().ForEach(pair => response.Headers.Append(pair.Key, pair.Value));
        }

        private async Task WriteBodyAsync(ResponseMessage responseMessage, IOwinResponse response)
        {
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
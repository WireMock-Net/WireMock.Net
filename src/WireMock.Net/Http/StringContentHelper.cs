using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using JetBrains.Annotations;
using MimeKit;
using WireMock.Validation;

namespace WireMock.Http
{
    internal static class StringContentHelper
    {
        /// <summary>
        /// Creates a StringContent object. Note that the Encoding is only set when it's also set on the original header.
        /// </summary>
        /// <param name="content">The string content (cannot be null)</param>
        /// <param name="contentType">The ContentType (can be null)</param>
        /// <returns>StringContent</returns>
        internal static StringContent Create([NotNull] string content, [CanBeNull] ContentType contentType)
        {
            Check.NotNull(content, nameof(content));

            if (contentType == null)
            {
                return new StringContent(content);
            }

            if (contentType.Charset == null)
            {
                var stringContent = new StringContent(content);
                stringContent.Headers.ContentType = new MediaTypeHeaderValue(contentType.MimeType);
                return stringContent;
            }

            var encoding = Encoding.GetEncoding(contentType.Charset);
            return new StringContent(content, encoding, contentType.MimeType);
        }
    }
}
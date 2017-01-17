using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;

[module:
    SuppressMessage("StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Justification = "Reviewed. Suppression is OK here, as it conflicts with internal naming rules.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
// ReSharper disable ArrangeThisQualifier
namespace WireMock
{
    /// <summary>
    /// The http listener request mapper.
    /// </summary>
    public class HttpListenerRequestMapper
    {
        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="listenerRequest">
        /// The listener request.
        /// </param>
        /// <returns>
        /// The <see cref="Request"/>.
        /// </returns>
        public Request Map(HttpListenerRequest listenerRequest)
        {
            var path = listenerRequest.Url.AbsolutePath;
            var query = listenerRequest.Url.Query;
            var verb = listenerRequest.HttpMethod;
            var body = GetRequestBody(listenerRequest);
            var listenerHeaders = listenerRequest.Headers;
            var headers = listenerHeaders.AllKeys.ToDictionary(k => k, k => listenerHeaders[k]);

            return new Request(path, query, verb, body, headers);
        }

        /// <summary>
        /// The get request body.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetRequestBody(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var body = request.InputStream)
            {
                using (var reader = new StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

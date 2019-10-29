using System.Net.Http;
using WireMock.Http;
using WireMock.Settings;
using WireMock.Validation;

namespace WireMock.ResponseBuilders
{
    public partial class Response
    {
        private HttpClient _httpClientForProxy;

        /// <inheritdoc cref="IProxyResponseBuilder.WithProxy(string, string)"/>
        public IResponseBuilder WithProxy(string proxyUrl, string clientX509Certificate2ThumbprintOrSubjectName = null)
        {
            Check.NotNullOrEmpty(proxyUrl, nameof(proxyUrl));

            var settings = new ProxyAndRecordSettings
            {
                Url = proxyUrl,
                ClientX509Certificate2ThumbprintOrSubjectName = clientX509Certificate2ThumbprintOrSubjectName
            };

            return WithProxy(settings);
        }

        /// <inheritdoc cref="IProxyResponseBuilder.WithProxy(IProxyAndRecordSettings)"/>
        public IResponseBuilder WithProxy(IProxyAndRecordSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            ProxyUrl = settings.Url;
            _httpClientForProxy = HttpClientHelper.CreateHttpClient(settings);
            return this;
        }
    }
}
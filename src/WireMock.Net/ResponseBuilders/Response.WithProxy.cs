using System.Net.Http;
using WireMock.Http;
using WireMock.Settings;
using Stef.Validation;

namespace WireMock.ResponseBuilders;

public partial class Response
{
    private HttpClient? _httpClientForProxy;

    /// <summary>
    /// The WebProxy settings.
    /// </summary>
    public ProxyAndRecordSettings? ProxyAndRecordSettings { get; private set; }

    /// <inheritdoc />
    public IResponseBuilder WithProxy(string proxyUrl, string? clientX509Certificate2ThumbprintOrSubjectName = null)
    {
        Guard.NotNullOrEmpty(proxyUrl);

        var settings = new ProxyAndRecordSettings
        {
            Url = proxyUrl,
            ClientX509Certificate2ThumbprintOrSubjectName = clientX509Certificate2ThumbprintOrSubjectName
        };

        return WithProxy(settings);
    }

    /// <inheritdoc />
    public IResponseBuilder WithProxy(ProxyAndRecordSettings settings)
    {
        Guard.NotNull(settings);

        ProxyAndRecordSettings = settings;

        _httpClientForProxy = HttpClientBuilder.Build(settings);
        return this;
    }
}
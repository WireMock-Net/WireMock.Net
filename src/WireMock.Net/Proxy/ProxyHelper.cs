using Stef.Validation;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Http;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Proxy;

internal class ProxyHelper
{
    private readonly WireMockServerSettings _settings;
    private readonly ProxyMappingConverter _proxyMappingConverter;

    public ProxyHelper(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
        _proxyMappingConverter = new ProxyMappingConverter(settings, new GuidUtils());
    }

    public async Task<(IResponseMessage Message, IMapping? Mapping)> SendAsync(
        IMapping? mapping,
        ProxyAndRecordSettings proxyAndRecordSettings,
        HttpClient client,
        IRequestMessage requestMessage,
        string url)
    {
        Guard.NotNull(client);
        Guard.NotNull(requestMessage);
        Guard.NotNull(url);

        var originalUri = new Uri(requestMessage.Url);
        var requiredUri = new Uri(url);

        // Create HttpRequestMessage
        var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, url);

        // Call the URL
        var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        // Create ResponseMessage
        bool deserializeJson = !_settings.DisableJsonBodyParsing.GetValueOrDefault(false);
        bool decompressGzipAndDeflate = !_settings.DisableRequestBodyDecompressing.GetValueOrDefault(false);

        var responseMessage = await HttpResponseMessageHelper.CreateAsync(httpResponseMessage, requiredUri, originalUri, deserializeJson, decompressGzipAndDeflate).ConfigureAwait(false);

        IMapping? newMapping = null;
        if (HttpStatusRangeParser.IsMatch(proxyAndRecordSettings.SaveMappingForStatusCodePattern, responseMessage.StatusCode) &&
            (proxyAndRecordSettings.SaveMapping || proxyAndRecordSettings.SaveMappingToFile))
        {
            newMapping = _proxyMappingConverter.ToMapping(mapping, proxyAndRecordSettings, requestMessage, responseMessage);
        }

        return (responseMessage, mapping);
    }

    private IMapping ToMapping(ProxyAndRecordSettings proxyAndRecordSettings, IRequestMessage requestMessage, ResponseMessage responseMessage)
    {
        var excludedHeaders = proxyAndRecordSettings.ExcludedHeaders ?? new string[] { };
        var excludedCookies = proxyAndRecordSettings.ExcludedCookies ?? new string[] { };

        var request = Request.Create();
        request.WithPath(requestMessage.Path);
        request.UsingMethod(requestMessage.Method);

        requestMessage.Query?.Loop((key, value) => request.WithParam(key, false, value.ToArray()));
        requestMessage.Cookies?.Loop((key, value) =>
        {
            if (!excludedCookies.Contains(key, StringComparer.OrdinalIgnoreCase))
            {
                request.WithCookie(key, value);
            }
        });

        var allExcludedHeaders = new List<string>(excludedHeaders) { "Cookie" };
        requestMessage.Headers?.Loop((key, value) =>
        {
            if (!allExcludedHeaders.Contains(key, StringComparer.OrdinalIgnoreCase))
            {
                request.WithHeader(key, value.ToArray());
            }
        });

        bool throwExceptionWhenMatcherFails = _settings.ThrowExceptionWhenMatcherFails == true;
        switch (requestMessage.BodyData?.DetectedBodyType)
        {
            case BodyType.Json:
                request.WithBody(new JsonMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyData.BodyAsJson!, true, throwExceptionWhenMatcherFails));
                break;

            case BodyType.String:
                request.WithBody(new ExactMatcher(MatchBehaviour.AcceptOnMatch, false, throwExceptionWhenMatcherFails, MatchOperator.Or, requestMessage.BodyData.BodyAsString));
                break;

            case BodyType.Bytes:
                request.WithBody(new ExactObjectMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyData.BodyAsBytes, throwExceptionWhenMatcherFails));
                break;
        }

        var response = Response.Create(responseMessage);

        return new Mapping
        (
            guid: Guid.NewGuid(),
            title: $"Proxy Mapping for {requestMessage.Method} {requestMessage.Path}",
            description: string.Empty,
            path: null,
            settings: _settings,
            request,
            response,
            priority: WireMockConstants.ProxyPriority, // This was 0
            scenario: null,
            executionConditionState: null,
            nextState: null,
            stateTimes: null,
            webhooks: null,
            useWebhooksFireAndForget: null,
            timeSettings: null
        );
    }
}
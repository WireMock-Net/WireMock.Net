using Newtonsoft.Json.Linq;
using Stef.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Constants;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Proxy;

internal class ProxyHelper
{
    private readonly WireMockServerSettings _settings;

    public ProxyHelper(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
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
            newMapping = ToMapping(mapping, proxyAndRecordSettings, requestMessage, responseMessage);
        }

        return (responseMessage, newMapping);
    }

    private IMapping ToMapping(IMapping? mapping, ProxyAndRecordSettings proxyAndRecordSettings, IRequestMessage requestMessage, ResponseMessage responseMessage)
    {
        var request = (Request?)mapping?.RequestMatcher;
        var clientIPMatcher = request?.GetRequestMessageMatcher<RequestMessageClientIPMatcher>();
        var pathMatcher = request?.GetRequestMessageMatcher<RequestMessagePathMatcher>();
        var urlMatcher = request?.GetRequestMessageMatcher<RequestMessageUrlMatcher>();
        var headerMatchers = request?.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
        var cookieMatchers = request?.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
        var paramMatchers = request?.GetRequestMessageMatchers<RequestMessageParamMatcher>();
        var methodMatcher = request?.GetRequestMessageMatcher<RequestMessageMethodMatcher>();
        var bodyMatcher = request?.GetRequestMessageMatcher<RequestMessageBodyMatcher>();

        var useDefinedRequestMatchers = proxyAndRecordSettings.UseDefinedRequestMatchers;

        var excludedHeaders = new List<string>(proxyAndRecordSettings.ExcludedHeaders ?? new string[] { }) { "Cookie" };
        var excludedCookies = proxyAndRecordSettings.ExcludedCookies ?? new string[] { };

        var newRequest = Request.Create();

        // Path
        if (useDefinedRequestMatchers && pathMatcher?.Matchers is not null)
        {
            newRequest.WithPath(pathMatcher.MatchOperator, pathMatcher.Matchers.ToArray());
        }
        else
        {
            newRequest.WithPath(requestMessage.Path);
        }

        // Method
        if (useDefinedRequestMatchers && methodMatcher is not null)
        {
            newRequest.UsingMethod(methodMatcher.Methods);
        }
        else
        {
            newRequest.UsingMethod(requestMessage.Method);
        }

        // QueryParams
        if (useDefinedRequestMatchers && paramMatchers is not null)
        {
            foreach (var paramMatcher in paramMatchers)
            {
                newRequest.WithParam(paramMatcher.Key, paramMatcher.MatchBehaviour, paramMatcher.Matchers.ToArray());
            }
        }
        else
        {
            requestMessage.Query?.Loop((key, value) => newRequest.WithParam(key, false, value.ToArray()));
        }

        // Cookies
        if (useDefinedRequestMatchers && cookieMatchers is not null)
        {
            foreach (var cookieMatcher in cookieMatchers)
            {
                if (!excludedCookies.Contains(cookieMatcher.Name, StringComparer.OrdinalIgnoreCase))
                {
                    newRequest.WithCookie(cookieMatcher.Name, cookieMatcher.Matchers);
                }
            }
        }
        else
        {
            requestMessage.Cookies?.Loop((key, value) =>
            {
                if (!excludedCookies.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    newRequest.WithCookie(key, value);
                }
            });
        }

        // Headers
        if (useDefinedRequestMatchers && headerMatchers is not null)
        {
            foreach (var headerMatcher in headerMatchers.Where(hm => hm.Matchers is not null))
            {
                if (!excludedHeaders.Contains(headerMatcher.Name, StringComparer.OrdinalIgnoreCase))
                {
                    newRequest.WithHeader(headerMatcher.Name, headerMatcher.Matchers!);
                }
            }
        }
        else
        {
            requestMessage.Headers?.Loop((key, value) =>
            {
                if (!excludedHeaders.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    newRequest.WithHeader(key, value.ToArray());
                }
            });
        }

        // Body
        bool throwExceptionWhenMatcherFails = _settings.ThrowExceptionWhenMatcherFails == true;
        if (useDefinedRequestMatchers && bodyMatcher?.Matchers is not null)
        {
            newRequest.WithBody(bodyMatcher.Matchers);
        }
        else
        {
            switch (requestMessage.BodyData?.DetectedBodyType)
            {
                case BodyType.Json:
                    newRequest.WithBody(new JsonMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyData.BodyAsJson!, true, throwExceptionWhenMatcherFails));
                    break;

                case BodyType.String:
                    newRequest.WithBody(new ExactMatcher(MatchBehaviour.AcceptOnMatch, throwExceptionWhenMatcherFails, MatchOperator.Or, requestMessage.BodyData.BodyAsString));
                    break;

                case BodyType.Bytes:
                    newRequest.WithBody(new ExactObjectMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyData.BodyAsBytes, throwExceptionWhenMatcherFails));
                    break;
            }
        }

        // Title
        var title = useDefinedRequestMatchers && !string.IsNullOrEmpty(mapping?.Title) ?
            mapping!.Title :
            $"Proxy Mapping for {requestMessage.Method} {requestMessage.Path}";

        // Description
        var description = useDefinedRequestMatchers && !string.IsNullOrEmpty(mapping?.Description) ?
            mapping!.Description :
            $"Proxy Mapping for {requestMessage.Method} {requestMessage.Path}";

        return new Mapping
        (
            guid: Guid.NewGuid(),
            title: title,
            description: description,
            path: null,
            settings: _settings,
            newRequest,
            Response.Create(responseMessage),
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
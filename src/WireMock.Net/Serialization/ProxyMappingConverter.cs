using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Constants;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Serialization;

internal class ProxyMappingConverter
{
    private readonly WireMockServerSettings _settings;
    private readonly IGuidUtils _guidUtils;
    private readonly IDateTimeUtils _dateTimeUtils;

    public ProxyMappingConverter(WireMockServerSettings settings, IGuidUtils guidUtils, IDateTimeUtils dateTimeUtils)
    {
        _settings = Guard.NotNull(settings);
        _guidUtils = Guard.NotNull(guidUtils);
        _dateTimeUtils = Guard.NotNull(dateTimeUtils);
    }

    public IMapping? ToMapping(IMapping? mapping, ProxyAndRecordSettings proxyAndRecordSettings, IRequestMessage requestMessage, ResponseMessage responseMessage)
    {
        var useDefinedRequestMatchers = proxyAndRecordSettings.UseDefinedRequestMatchers;
        var excludedHeaders = new List<string>(proxyAndRecordSettings.ExcludedHeaders ?? new string[] { }) { "Cookie" };
        var excludedCookies = proxyAndRecordSettings.ExcludedCookies ?? new string[0];

        var request = (Request?)mapping?.RequestMatcher;
        var clientIPMatcher = request?.GetRequestMessageMatcher<RequestMessageClientIPMatcher>();
        var pathMatcher = request?.GetRequestMessageMatcher<RequestMessagePathMatcher>();
        var headerMatchers = request?.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
        var cookieMatchers = request?.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
        var paramMatchers = request?.GetRequestMessageMatchers<RequestMessageParamMatcher>();
        var methodMatcher = request?.GetRequestMessageMatcher<RequestMessageMethodMatcher>();
        var bodyMatcher = request?.GetRequestMessageMatcher<RequestMessageBodyMatcher>();

        //if (useDefinedRequestMatchers && excludedHttpMethods.Any())
        //{
        //    if (methodMather != null && methodMather.MatchBehaviour == MatchBehaviour.AcceptOnMatch)
        //    {
        //        foreach (var httpMethod in methodMather.Methods)
        //        {
        //            if (excludedHttpMethods.Contains(httpMethod, StringComparer.OrdinalIgnoreCase))
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (excludedHttpMethods.Contains("GET", StringComparer.OrdinalIgnoreCase))
        //        {
        //            return null;
        //        }
        //    }
        //}

        var newRequest = Request.Create();

        // ClientIP
        if (useDefinedRequestMatchers && clientIPMatcher?.Matchers is not null)
        {
            newRequest.WithClientIP(clientIPMatcher.MatchOperator, clientIPMatcher.Matchers.ToArray());
        }

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
                newRequest.WithParam(paramMatcher.Key, paramMatcher.MatchBehaviour, paramMatcher.Matchers!.ToArray());
            }
        }
        else
        {
            requestMessage.Query?.Loop((key, value) => newRequest.WithParam(key, false, value.ToArray()));
        }

        // Cookies
        if (useDefinedRequestMatchers && cookieMatchers is not null)
        {
            foreach (var cookieMatcher in cookieMatchers.Where(hm => hm.Matchers is not null))
            {
                if (!excludedCookies.Contains(cookieMatcher.Name, StringComparer.OrdinalIgnoreCase))
                {
                    newRequest.WithCookie(cookieMatcher.Name, cookieMatcher.Matchers!);
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
                    newRequest.WithBody(new ExactMatcher(MatchBehaviour.AcceptOnMatch, true, throwExceptionWhenMatcherFails, MatchOperator.Or, requestMessage.BodyData.BodyAsString!));
                    break;

                case BodyType.Bytes:
                    newRequest.WithBody(new ExactObjectMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyData.BodyAsBytes!, throwExceptionWhenMatcherFails));
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
            guid: _guidUtils.NewGuid(),
            updatedAt: _dateTimeUtils.UtcNow,
            title: title,
            description: description,
            path: null,
            settings: _settings,
            requestMatcher: newRequest,
            provider: Response.Create(responseMessage),
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
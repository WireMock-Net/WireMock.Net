﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HandlebarsDotNet.Helpers.Validation;
using JetBrains.Annotations;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Proxy
{
    internal class ProxyHelper
    {
        private readonly IWireMockServerSettings _settings;

        public ProxyHelper([NotNull] IWireMockServerSettings settings)
        {
            Guard.NotNull(settings, nameof(settings));
            _settings = settings;
        }

        public async Task<(ResponseMessage ResponseMessage, IMapping Mapping)> SendAsync(
            [NotNull] HttpClient client,
            [NotNull] RequestMessage requestMessage,
            [NotNull] string url)
        {
            Guard.NotNull(client, nameof(client));
            Guard.NotNull(requestMessage, nameof(requestMessage));
            Guard.NotNull(url, nameof(url));

            var originalUri = new Uri(requestMessage.Url);
            var requiredUri = new Uri(url);

            // Create HttpRequestMessage
            var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, url);

            // Call the URL
            var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead);

            // Create ResponseMessage
            bool deserializeJson = !_settings.DisableJsonBodyParsing.GetValueOrDefault(false);
            bool decompressGzipAndDeflate = !_settings.DisableRequestBodyDecompressing.GetValueOrDefault(false);

            var responseMessage = await HttpResponseMessageHelper.CreateAsync(httpResponseMessage, requiredUri, originalUri, deserializeJson, decompressGzipAndDeflate);

            IMapping mapping = null;
            if (HttpStatusRangeParser.IsMatch(_settings.ProxyAndRecordSettings.SaveMappingForStatusCodePattern, responseMessage.StatusCode) &&
                (_settings.ProxyAndRecordSettings.SaveMapping || _settings.ProxyAndRecordSettings.SaveMappingToFile))
            {
                mapping = ToMapping(requestMessage, responseMessage);
            }

            return (responseMessage, mapping);
        }

        private IMapping ToMapping(RequestMessage requestMessage, ResponseMessage responseMessage)
        {
            string[] excludedHeaders = _settings.ProxyAndRecordSettings.ExcludedHeaders ?? new string[] { };
            string[] excludedCookies = _settings.ProxyAndRecordSettings.ExcludedCookies ?? new string[] { };

            var request = Request.Create();
            request.WithPath(requestMessage.Path);
            request.UsingMethod(requestMessage.Method);

            requestMessage.Query.Loop((key, value) => request.WithParam(key, false, value.ToArray()));
            requestMessage.Cookies.Loop((key, value) =>
            {
                if (!excludedCookies.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    request.WithCookie(key, value);
                }
            });

            var allExcludedHeaders = new List<string>(excludedHeaders) { "Cookie" };
            requestMessage.Headers.Loop((key, value) =>
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
                    request.WithBody(new JsonMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyData.BodyAsJson, true, throwExceptionWhenMatcherFails));
                    break;

                case BodyType.String:
                    request.WithBody(new ExactMatcher(MatchBehaviour.AcceptOnMatch, throwExceptionWhenMatcherFails, requestMessage.BodyData.BodyAsString));
                    break;

                case BodyType.Bytes:
                    request.WithBody(new ExactObjectMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyData.BodyAsBytes, throwExceptionWhenMatcherFails));
                    break;
            }

            var response = Response.Create(responseMessage);

            return new Mapping(Guid.NewGuid(), string.Empty, null, _settings, request, response, 0, null, null, null, null);
        }
    }
}
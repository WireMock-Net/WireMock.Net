using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Server;

public partial class WireMockServer
{
    private void ConvertMappingsAndRegisterAsRespondProvider(IReadOnlyList<MappingModel> mappingModels, string? path = null)
    {
        var duplicateGuids = mappingModels
            .Where(m => m.Guid != null)
            .GroupBy(m => m.Guid)
            .Where(g => g.Count() > 1)
            .Select(g => $"'{g.Key}'")
            .ToArray();
        if (duplicateGuids.Any())
        {
            throw new ArgumentException($"The following Guids are duplicate : {string.Join(",", duplicateGuids)}", nameof(mappingModels));
        }

        foreach (var mappingModel in mappingModels)
        {
            ConvertMappingAndRegisterAsRespondProvider(mappingModel, null, path);
        }
    }

    private Guid? ConvertMappingAndRegisterAsRespondProvider(MappingModel mappingModel, Guid? guid = null, string? path = null)
    {
        Guard.NotNull(mappingModel);
        Guard.NotNull(mappingModel.Request, nameof(mappingModel.Request));
        Guard.NotNull(mappingModel.Response, nameof(mappingModel.Response));

        var requestBuilder = InitRequestBuilder(mappingModel.Request, true);
        if (requestBuilder == null)
        {
            return null;
        }

        var respondProvider = Given(requestBuilder, mappingModel.SaveToFile == true);

        if (guid != null)
        {
            respondProvider = respondProvider.WithGuid(guid.Value);
        }
        else if (mappingModel.Guid != null && mappingModel.Guid != Guid.Empty)
        {
            respondProvider = respondProvider.WithGuid(mappingModel.Guid.Value);
        }

        if (mappingModel.Data != null)
        {
            respondProvider = respondProvider.WithData(mappingModel.Data);
        }

        var timeSettings = TimeSettingsMapper.Map(mappingModel.TimeSettings);
        if (timeSettings != null)
        {
            respondProvider = respondProvider.WithTimeSettings(timeSettings);
        }

        if (path != null)
        {
            respondProvider = respondProvider.WithPath(path);
        }

        if (!string.IsNullOrEmpty(mappingModel.Title))
        {
            respondProvider = respondProvider.WithTitle(mappingModel.Title!);
        }

        if (mappingModel.Priority != null)
        {
            respondProvider = respondProvider.AtPriority(mappingModel.Priority.Value);
        }

        if (mappingModel.Scenario != null)
        {
            respondProvider = respondProvider.InScenario(mappingModel.Scenario);

            if (!string.IsNullOrEmpty(mappingModel.WhenStateIs))
            {
                respondProvider = respondProvider.WhenStateIs(mappingModel.WhenStateIs!);
            }

            if (!string.IsNullOrEmpty(mappingModel.SetStateTo))
            {
                respondProvider = respondProvider.WillSetStateTo(mappingModel.SetStateTo!);
            }
        }

        if (mappingModel.Webhook != null)
        {
            respondProvider = respondProvider.WithWebhook(WebhookMapper.Map(mappingModel.Webhook));
        }
        else if (mappingModel.Webhooks?.Length > 1)
        {
            var webhooks = mappingModel.Webhooks.Select(WebhookMapper.Map).ToArray();
            respondProvider = respondProvider.WithWebhook(webhooks);
        }

        if (mappingModel.UseWebhooksFireAndForget == true)
        {
            respondProvider.WithWebhookFireAndForget(mappingModel.UseWebhooksFireAndForget.Value);
        }

        if (mappingModel.Probability != null)
        {
            respondProvider.WithProbability(mappingModel.Probability.Value);
        }

        var responseBuilder = InitResponseBuilder(mappingModel.Response);
        respondProvider.RespondWith(responseBuilder);

        return respondProvider.Guid;
    }

    private IRequestBuilder? InitRequestBuilder(RequestModel requestModel, bool pathOrUrlRequired)
    {
        IRequestBuilder requestBuilder = Request.Create();

        if (requestModel.ClientIP != null)
        {
            if (requestModel.ClientIP is string clientIP)
            {
                requestBuilder = requestBuilder.WithClientIP(clientIP);
            }
            else
            {
                var clientIPModel = JsonUtils.ParseJTokenToObject<ClientIPModel>(requestModel.ClientIP);
                if (clientIPModel.Matchers != null)
                {
                    requestBuilder = requestBuilder.WithPath(clientIPModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
                }
            }
        }

        bool pathOrUrlMatchersValid = false;
        if (requestModel.Path != null)
        {
            if (requestModel.Path is string path)
            {
                requestBuilder = requestBuilder.WithPath(path);
                pathOrUrlMatchersValid = true;
            }
            else
            {
                var pathModel = JsonUtils.ParseJTokenToObject<PathModel>(requestModel.Path);
                if (pathModel.Matchers != null)
                {
                    var matchOperator = StringUtils.ParseMatchOperator(pathModel.MatchOperator);
                    requestBuilder = requestBuilder.WithPath(matchOperator, pathModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
                    pathOrUrlMatchersValid = true;
                }
            }
        }
        else if (requestModel.Url != null)
        {
            if (requestModel.Url is string url)
            {
                requestBuilder = requestBuilder.WithUrl(url);
                pathOrUrlMatchersValid = true;
            }
            else
            {
                var urlModel = JsonUtils.ParseJTokenToObject<UrlModel>(requestModel.Url);
                if (urlModel.Matchers != null)
                {
                    var matchOperator = StringUtils.ParseMatchOperator(urlModel.MatchOperator);
                    requestBuilder = requestBuilder.WithUrl(matchOperator, urlModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
                    pathOrUrlMatchersValid = true;
                }
            }
        }

        if (pathOrUrlRequired && !pathOrUrlMatchersValid)
        {
            _settings.Logger.Error("Path or Url matcher is missing for this mapping, this mapping will not be added.");
            return null;
        }

        if (requestModel.Methods != null)
        {
            var matchBehaviour = requestModel.MethodsRejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch;
            var matchOperator = StringUtils.ParseMatchOperator(requestModel.MethodsMatchOperator);
            requestBuilder = requestBuilder.UsingMethod(matchBehaviour, matchOperator, requestModel.Methods);
        }

        if (requestModel.Headers != null)
        {
            foreach (var headerModel in requestModel.Headers.Where(h => h.Matchers != null))
            {
                var matchOperator = StringUtils.ParseMatchOperator(headerModel.MatchOperator);
                requestBuilder = requestBuilder.WithHeader(
                    headerModel.Name,
                    headerModel.IgnoreCase == true,
                    headerModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                    matchOperator,
                    headerModel.Matchers!.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray()
                );
            }
        }

        if (requestModel.Cookies != null)
        {
            foreach (var cookieModel in requestModel.Cookies.Where(c => c.Matchers != null))
            {
                requestBuilder = requestBuilder.WithCookie(
                     cookieModel.Name,
                     cookieModel.IgnoreCase == true,
                     cookieModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                     cookieModel.Matchers!.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
            }
        }

        if (requestModel.Params != null)
        {
            foreach (var paramModel in requestModel.Params.Where(p => p is { Matchers: { } }))
            {
                bool ignoreCase = paramModel.IgnoreCase == true;
                requestBuilder = requestBuilder.WithParam(paramModel.Name, ignoreCase, paramModel.Matchers!.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
            }
        }

        if (requestModel.Body?.Matcher != null)
        {
            requestBuilder = requestBuilder.WithBody(_matcherMapper.Map(requestModel.Body.Matcher)!);
        }
        else if (requestModel.Body?.Matchers != null)
        {
            var matchOperator = StringUtils.ParseMatchOperator(requestModel.Body.MatchOperator);
            requestBuilder = requestBuilder.WithBody(_matcherMapper.Map(requestModel.Body.Matchers)!, matchOperator);
        }

        return requestBuilder;
    }

    private static IResponseBuilder InitResponseBuilder(ResponseModel responseModel)
    {
        IResponseBuilder responseBuilder = Response.Create();

        if (responseModel.Delay > 0)
        {
            responseBuilder = responseBuilder.WithDelay(responseModel.Delay.Value);
        }
        else if (responseModel.MinimumRandomDelay >= 0 || responseModel.MaximumRandomDelay > 0)
        {
            responseBuilder = responseBuilder.WithRandomDelay(responseModel.MinimumRandomDelay ?? 0, responseModel.MaximumRandomDelay ?? 60_000);
        }

        if (responseModel.UseTransformer == true)
        {
            if (!Enum.TryParse<TransformerType>(responseModel.TransformerType, out var transformerType))
            {
                transformerType = TransformerType.Handlebars;
            }

            if (!Enum.TryParse<ReplaceNodeOptions>(responseModel.TransformerReplaceNodeOptions, out var replaceNodeOptions))
            {
                replaceNodeOptions = ReplaceNodeOptions.EvaluateAndTryToConvert;
            }

            responseBuilder = responseBuilder.WithTransformer(
                transformerType,
                responseModel.UseTransformerForBodyAsFile == true,
                replaceNodeOptions
            );
        }

        if (!string.IsNullOrEmpty(responseModel.ProxyUrl))
        {
            var proxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = responseModel.ProxyUrl!,
                ClientX509Certificate2ThumbprintOrSubjectName = responseModel.X509Certificate2ThumbprintOrSubjectName,
                WebProxySettings = TinyMapperUtils.Instance.Map(responseModel.WebProxy),
                ReplaceSettings = TinyMapperUtils.Instance.Map(responseModel.ProxyUrlReplaceSettings)
            };

            return responseBuilder.WithProxy(proxyAndRecordSettings);
        }

        if (responseModel.StatusCode is string statusCodeAsString)
        {
            responseBuilder = responseBuilder.WithStatusCode(statusCodeAsString);
        }
        else if (responseModel.StatusCode != null)
        {
            // Convert to Int32 because Newtonsoft deserializes an 'object' with a number value to a long.
            responseBuilder = responseBuilder.WithStatusCode(Convert.ToInt32(responseModel.StatusCode));
        }

        if (responseModel.Headers != null)
        {
            foreach (var entry in responseModel.Headers)
            {
                if (entry.Value is string value)
                {
                    responseBuilder.WithHeader(entry.Key, value);
                }
                else
                {
                    var headers = JsonUtils.ParseJTokenToObject<string[]>(entry.Value);
                    responseBuilder.WithHeader(entry.Key, headers);
                }
            }
        }
        else if (responseModel.HeadersRaw != null)
        {
            foreach (string headerLine in responseModel.HeadersRaw.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                int indexColon = headerLine.IndexOf(":", StringComparison.Ordinal);
                string key = headerLine.Substring(0, indexColon).TrimStart(' ', '\t');
                string value = headerLine.Substring(indexColon + 1).TrimStart(' ', '\t');
                responseBuilder = responseBuilder.WithHeader(key, value);
            }
        }

        if (responseModel.BodyAsBytes != null)
        {
            responseBuilder = responseBuilder.WithBody(responseModel.BodyAsBytes, responseModel.BodyDestination, ToEncoding(responseModel.BodyEncoding));
        }
        else if (responseModel.Body != null)
        {
            responseBuilder = responseBuilder.WithBody(responseModel.Body, responseModel.BodyDestination, ToEncoding(responseModel.BodyEncoding));
        }
        else if (responseModel.BodyAsJson != null)
        {
            responseBuilder = responseBuilder.WithBodyAsJson(responseModel.BodyAsJson, ToEncoding(responseModel.BodyEncoding), responseModel.BodyAsJsonIndented == true);
        }
        else if (responseModel.BodyAsFile != null)
        {
            responseBuilder = responseBuilder.WithBodyFromFile(responseModel.BodyAsFile, responseModel.BodyAsFileIsCached == true);
        }

        if (responseModel.Fault != null && Enum.TryParse(responseModel.Fault.Type, out FaultType faultType))
        {
            responseBuilder.WithFault(faultType, responseModel.Fault.Percentage);
        }

        return responseBuilder;
    }
}
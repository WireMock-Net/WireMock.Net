using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Constants;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Serialization;

internal class MappingConverter
{
    private static readonly string AcceptOnMatch = MatchBehaviour.AcceptOnMatch.GetFullyQualifiedEnumValue();

    private readonly MatcherMapper _mapper;

    public MappingConverter(MatcherMapper mapper)
    {
        _mapper = Guard.NotNull(mapper);
    }

    public string ToCSharpCode(IMapping mapping, MappingConverterSettings? settings = null)
    {
        settings ??= new MappingConverterSettings();

        var request = (Request)mapping.RequestMatcher;
        var response = (Response)mapping.Provider;

        var clientIPMatcher = request.GetRequestMessageMatcher<RequestMessageClientIPMatcher>();
        var pathMatcher = request.GetRequestMessageMatcher<RequestMessagePathMatcher>();
        var urlMatcher = request.GetRequestMessageMatcher<RequestMessageUrlMatcher>();
        var headerMatchers = request.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
        var cookieMatchers = request.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
        var paramsMatchers = request.GetRequestMessageMatchers<RequestMessageParamMatcher>();
        var methodMatcher = request.GetRequestMessageMatcher<RequestMessageMethodMatcher>();
        var bodyMatcher = request.GetRequestMessageMatcher<RequestMessageBodyMatcher>();

        var sb = new StringBuilder();

        if (settings.ConverterType == MappingConverterType.Server)
        {
            if (settings.AddStart)
            {
                sb.AppendLine("var server = WireMockServer.Start();");
            }
            sb.AppendLine("server");
        }
        else
        {
            if (settings.AddStart)
            {
                sb.AppendLine("var builder = new MappingBuilder();");
            }
            sb.AppendLine("builder");
        }

        // Request
        sb.AppendLine("    .Given(Request.Create()");
        sb.AppendLine($"        .UsingMethod({To1Or2Or3Arguments(methodMatcher?.MatchBehaviour, methodMatcher?.MatchOperator, methodMatcher?.Methods, HttpRequestMethod.GET)})");

        if (pathMatcher is { Matchers: { } })
        {
            sb.AppendLine($"        .WithPath({To1Or2Arguments(pathMatcher.MatchOperator, pathMatcher.Matchers)})");
        }
        else if (urlMatcher is { Matchers: { } })
        {
            sb.AppendLine($"        .WithUrl({To1Or2Arguments(urlMatcher.MatchOperator, urlMatcher.Matchers)})");
        }

        foreach (var paramsMatcher in paramsMatchers)
        {
            sb.AppendLine($"        .WithParam({To1Or2Or3Arguments(paramsMatcher.Key, paramsMatcher.MatchBehaviour, paramsMatcher.Matchers!)})");
        }

        if (clientIPMatcher is { Matchers: { } })
        {
            sb.AppendLine($"        .WithClientIP({ToValueArguments(GetStringArray(clientIPMatcher.Matchers))})");
        }

        foreach (var headerMatcher in headerMatchers.Where(h => h.Matchers is { }))
        {
            var headerBuilder = new StringBuilder($"\"{headerMatcher.Name}\", {ToValueArguments(GetStringArray(headerMatcher.Matchers!))}, true");
            if (headerMatcher.MatchOperator != MatchOperator.Or)
            {
                headerBuilder.Append($"{AcceptOnMatch}, {headerMatcher.MatchOperator.GetFullyQualifiedEnumValue()}");
            }
            sb.AppendLine($"        .WithHeader({headerBuilder})");
        }

        foreach (var cookieMatcher in cookieMatchers.Where(h => h.Matchers is { }))
        {
            sb.AppendLine($"        .WithCookie(\"{cookieMatcher.Name}\", {ToValueArguments(GetStringArray(cookieMatcher.Matchers!))}, true)");
        }

        if (bodyMatcher is { Matchers: { } })
        {
            var wildcardMatcher = bodyMatcher.Matchers.OfType<WildcardMatcher>().FirstOrDefault();
            if (wildcardMatcher is { } && wildcardMatcher.GetPatterns().Any())
            {
                sb.AppendLine($"        .WithBody({GetString(wildcardMatcher)})");
            }
        }

        sb.AppendLine(@"    )");

        // Guid
        sb.AppendLine($"    .WithGuid(\"{mapping.Guid}\")");

        if (mapping.Probability != null)
        {
            sb.AppendLine($"    .WithProbability({mapping.Probability.Value.ToString(CultureInfoUtils.CultureInfoEnUS)})");
        }

        // Response
        sb.AppendLine("    .RespondWith(Response.Create()");

        if (response.ResponseMessage.Headers is { })
        {
            foreach (var header in response.ResponseMessage.Headers)
            {
                sb.AppendLine($"        .WithHeader(\"{header.Key}\", {ToValueArguments(header.Value.ToArray())})");
            }
        }

        if (response.ResponseMessage.BodyData is { })
        {
            switch (response.ResponseMessage.BodyData.DetectedBodyType)
            {
                case BodyType.String:
                case BodyType.FormUrlEncoded:
                    sb.AppendLine($"        .WithBody(\"{response.ResponseMessage.BodyData.BodyAsString}\")");
                    break;
            }
        }

        if (response.Delay is { })
        {
            sb.AppendLine($"        .WithDelay({response.Delay.Value.TotalMilliseconds})");
        }
        else if (response is { MinimumDelayMilliseconds: > 0, MaximumDelayMilliseconds: > 0 })
        {
            sb.AppendLine($"        .WithRandomDelay({response.MinimumDelayMilliseconds}, {response.MaximumDelayMilliseconds})");
        }

        if (response.UseTransformer)
        {
            var transformerArgs = response.TransformerType != TransformerType.Handlebars ? response.TransformerType.GetFullyQualifiedEnumValue() : string.Empty;
            sb.AppendLine($"        .WithTransformer({transformerArgs})");
        }
        sb.AppendLine(@"    );");

        return sb.ToString();
    }

    public MappingModel ToMappingModel(IMapping mapping)
    {
        var request = (Request)mapping.RequestMatcher;
        var response = (Response)mapping.Provider;

        var clientIPMatcher = request.GetRequestMessageMatcher<RequestMessageClientIPMatcher>();
        var pathMatcher = request.GetRequestMessageMatcher<RequestMessagePathMatcher>();
        var urlMatcher = request.GetRequestMessageMatcher<RequestMessageUrlMatcher>();
        var headerMatchers = request.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
        var cookieMatchers = request.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
        var paramsMatchers = request.GetRequestMessageMatchers<RequestMessageParamMatcher>();
        var methodMatcher = request.GetRequestMessageMatcher<RequestMessageMethodMatcher>();
        var bodyMatcher = request.GetRequestMessageMatcher<RequestMessageBodyMatcher>();

        var mappingModel = new MappingModel
        {
            Guid = mapping.Guid,
            UpdatedAt = mapping.UpdatedAt,
            TimeSettings = TimeSettingsMapper.Map(mapping.TimeSettings),
            Title = mapping.Title,
            Description = mapping.Description,
            UseWebhooksFireAndForget = mapping.UseWebhooksFireAndForget,
            Priority = mapping.Priority != 0 ? mapping.Priority : null,
            Scenario = mapping.Scenario,
            WhenStateIs = mapping.ExecutionConditionState,
            SetStateTo = mapping.NextState,
            Data = mapping.Data,
            Probability = mapping.Probability,
            Request = new RequestModel
            {
                Headers = headerMatchers.Any() ? headerMatchers.Select(hm => new HeaderModel
                {
                    Name = hm.Name,
                    Matchers = _mapper.Map(hm.Matchers)
                }).ToList() : null,

                Cookies = cookieMatchers.Any() ? cookieMatchers.Select(cm => new CookieModel
                {
                    Name = cm.Name,
                    Matchers = _mapper.Map(cm.Matchers)
                }).ToList() : null,

                Params = paramsMatchers.Any() ? paramsMatchers.Select(pm => new ParamModel
                {
                    Name = pm.Key,
                    IgnoreCase = pm.IgnoreCase == true ? true : null,
                    Matchers = _mapper.Map(pm.Matchers)
                }).ToList() : null
            },
            Response = new ResponseModel()
        };

        if (methodMatcher is { Methods: { } })
        {
            mappingModel.Request.Methods = methodMatcher.Methods;
            mappingModel.Request.MethodsRejectOnMatch = methodMatcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : null;
            mappingModel.Request.MethodsMatchOperator = methodMatcher.Methods.Length > 1 ? methodMatcher.MatchOperator.ToString() : null;
        }

        if (clientIPMatcher is { Matchers: { } })
        {
            var clientIPMatchers = _mapper.Map(clientIPMatcher.Matchers);
            mappingModel.Request.Path = new ClientIPModel
            {
                Matchers = clientIPMatchers,
                MatchOperator = clientIPMatchers?.Length > 1 ? clientIPMatcher.MatchOperator.ToString() : null
            };
        }

        if (pathMatcher is { Matchers: { } })
        {
            var pathMatchers = _mapper.Map(pathMatcher.Matchers);
            mappingModel.Request.Path = new PathModel
            {
                Matchers = pathMatchers,
                MatchOperator = pathMatchers?.Length > 1 ? pathMatcher.MatchOperator.ToString() : null
            };
        }
        else if (urlMatcher is { Matchers: { } })
        {
            var urlMatchers = _mapper.Map(urlMatcher.Matchers);
            mappingModel.Request.Url = new UrlModel
            {
                Matchers = urlMatchers,
                MatchOperator = urlMatchers?.Length > 1 ? urlMatcher.MatchOperator.ToString() : null
            };
        }

        if (response.MinimumDelayMilliseconds >= 0 || response.MaximumDelayMilliseconds > 0)
        {
            mappingModel.Response.MinimumRandomDelay = response.MinimumDelayMilliseconds;
            mappingModel.Response.MaximumRandomDelay = response.MaximumDelayMilliseconds;
        }
        else
        {
            mappingModel.Response.Delay = (int?)(response.Delay == Timeout.InfiniteTimeSpan ? TimeSpan.MaxValue.TotalMilliseconds : response.Delay?.TotalMilliseconds);
        }

        var nonNullableWebHooks = mapping.Webhooks?.Where(wh => wh != null).ToArray() ?? new IWebhook[0];
        if (nonNullableWebHooks.Length == 1)
        {
            mappingModel.Webhook = WebhookMapper.Map(nonNullableWebHooks[0]);
        }
        else if (mapping.Webhooks?.Length > 1)
        {
            mappingModel.Webhooks = mapping.Webhooks.Select(WebhookMapper.Map).ToArray();
        }

        if (bodyMatcher?.Matchers != null)
        {
            mappingModel.Request.Body = new BodyModel();

            if (bodyMatcher.Matchers.Length == 1)
            {
                mappingModel.Request.Body.Matcher = _mapper.Map(bodyMatcher.Matchers[0]);
            }
            else if (bodyMatcher.Matchers.Length > 1)
            {
                mappingModel.Request.Body.Matchers = _mapper.Map(bodyMatcher.Matchers);
                mappingModel.Request.Body.MatchOperator = bodyMatcher.MatchOperator.ToString();
            }
        }

        if (response.ProxyAndRecordSettings != null)
        {
            mappingModel.Response.StatusCode = null;
            mappingModel.Response.Headers = null;
            mappingModel.Response.BodyDestination = null;
            mappingModel.Response.BodyAsJson = null;
            mappingModel.Response.BodyAsJsonIndented = null;
            mappingModel.Response.Body = null;
            mappingModel.Response.BodyAsBytes = null;
            mappingModel.Response.BodyAsFile = null;
            mappingModel.Response.BodyAsFileIsCached = null;
            mappingModel.Response.UseTransformer = null;
            mappingModel.Response.TransformerType = null;
            mappingModel.Response.UseTransformerForBodyAsFile = null;
            mappingModel.Response.TransformerReplaceNodeOptions = null;
            mappingModel.Response.BodyEncoding = null;
            mappingModel.Response.ProxyUrl = response.ProxyAndRecordSettings.Url;
            mappingModel.Response.Fault = null;
            mappingModel.Response.WebProxy = MapWebProxy(response.ProxyAndRecordSettings.WebProxySettings);
        }
        else
        {
            mappingModel.Response.WebProxy = null;
            mappingModel.Response.BodyDestination = response.ResponseMessage.BodyDestination;
            mappingModel.Response.StatusCode = response.ResponseMessage.StatusCode;

            if (response.ResponseMessage.Headers is { Count: > 0 })
            {
                mappingModel.Response.Headers = MapHeaders(response.ResponseMessage.Headers);
            }

            if (response.UseTransformer)
            {
                mappingModel.Response.UseTransformer = response.UseTransformer;
                mappingModel.Response.TransformerType = response.TransformerType.ToString();
                mappingModel.Response.TransformerReplaceNodeOptions = response.TransformerReplaceNodeOptions.ToString();
            }

            if (response.UseTransformerForBodyAsFile)
            {
                mappingModel.Response.UseTransformerForBodyAsFile = response.UseTransformerForBodyAsFile;
            }

            if (response.ResponseMessage.BodyData != null)
            {
                switch (response.ResponseMessage.BodyData?.DetectedBodyType)
                {
                    case BodyType.String:
                    case BodyType.FormUrlEncoded:
                        mappingModel.Response.Body = response.ResponseMessage.BodyData.BodyAsString;
                        break;

                    case BodyType.Json:
                        mappingModel.Response.BodyAsJson = response.ResponseMessage.BodyData.BodyAsJson;
                        if (response.ResponseMessage.BodyData.BodyAsJsonIndented == true)
                        {
                            mappingModel.Response.BodyAsJsonIndented = response.ResponseMessage.BodyData.BodyAsJsonIndented;
                        }
                        break;

                    case BodyType.Bytes:
                        mappingModel.Response.BodyAsBytes = response.ResponseMessage.BodyData.BodyAsBytes;
                        break;

                    case BodyType.File:
                        mappingModel.Response.BodyAsFile = response.ResponseMessage.BodyData.BodyAsFile;
                        mappingModel.Response.BodyAsFileIsCached = response.ResponseMessage.BodyData.BodyAsFileIsCached;
                        break;
                }

                if (response.ResponseMessage.BodyData?.Encoding != null && response.ResponseMessage.BodyData.Encoding.WebName != "utf-8")
                {
                    mappingModel.Response.BodyEncoding = new EncodingModel
                    {
                        EncodingName = response.ResponseMessage.BodyData.Encoding.EncodingName,
                        CodePage = response.ResponseMessage.BodyData.Encoding.CodePage,
                        WebName = response.ResponseMessage.BodyData.Encoding.WebName
                    };
                }
            }

            if (response.ResponseMessage.FaultType != FaultType.NONE)
            {
                mappingModel.Response.Fault = new FaultModel
                {
                    Type = response.ResponseMessage.FaultType.ToString(),
                    Percentage = response.ResponseMessage.FaultPercentage
                };
            }
        }

        return mappingModel;
    }

    private static string GetString(IStringMatcher stringMatcher)
    {
        return stringMatcher.GetPatterns().Select(p => $"\"{p.GetPattern()}\"").First();
    }

    private static string[] GetStringArray(IReadOnlyList<IStringMatcher> stringMatchers)
    {
        return stringMatchers.SelectMany(m => m.GetPatterns()).Select(p => p.GetPattern()).ToArray();
    }

    private static string To1Or2Or3Arguments(string key, MatchBehaviour? matchBehaviour, IReadOnlyList<IStringMatcher> matchers)
    {
        var sb = new StringBuilder($"\"{key}\", ");

        if (matchBehaviour.HasValue && matchBehaviour != MatchBehaviour.AcceptOnMatch)
        {
            sb.AppendFormat("{0}, ", matchBehaviour.Value.GetFullyQualifiedEnumValue());
        }

        sb.AppendFormat("{0}", ToValueArguments(GetStringArray(matchers), string.Empty));

        return sb.ToString();
    }

    private static string To1Or2Or3Arguments(MatchBehaviour? matchBehaviour, MatchOperator? matchOperator, string[]? values, string defaultValue)
    {
        var sb = new StringBuilder();

        if (matchBehaviour.HasValue && matchBehaviour != MatchBehaviour.AcceptOnMatch)
        {
            sb.AppendFormat("{0}, ", matchBehaviour.Value.GetFullyQualifiedEnumValue());
        }

        return To1Or2Arguments(matchOperator, values, defaultValue);
    }

    private static string To1Or2Arguments(MatchOperator? matchOperator, IReadOnlyList<IStringMatcher> matchers)
    {
        return To1Or2Arguments(matchOperator, GetStringArray(matchers), string.Empty);
    }

    private static string To1Or2Arguments(MatchOperator? matchOperator, string[]? values, string defaultValue)
    {
        var sb = new StringBuilder();

        if (matchOperator.HasValue && matchOperator != MatchOperator.Or)
        {
            sb.AppendFormat("{0}, ", matchOperator.Value.GetFullyQualifiedEnumValue());
        }

        return sb.Append(ToValueArguments(values, defaultValue)).ToString();
    }

    private static string ToValueArguments(string[]? values, string defaultValue = "")
    {
        return values is { } ? string.Join(", ", values.Select(v => $"\"{v}\"")) : $"\"{defaultValue}\"";
    }

    private static WebProxyModel? MapWebProxy(WebProxySettings? settings)
    {
        return settings != null ? new WebProxyModel
        {
            Address = settings.Address,
            UserName = settings.UserName,
            Password = settings.Password
        } : null;
    }

    private static IDictionary<string, object> MapHeaders(IDictionary<string, WireMockList<string>> dictionary)
    {
        var newDictionary = new Dictionary<string, object>();
        foreach (var entry in dictionary)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (entry.Value.Count == 1)
            {
                newDictionary.Add(entry.Key, entry.Value.ToString());
            }
            else
            {
                newDictionary.Add(entry.Key, entry.Value);
            }
        }

        return newDictionary;
    }
}
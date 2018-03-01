﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SimMetrics.Net;
using WireMock.Admin.Mappings;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Util;

namespace WireMock.Serialization
{
    internal static class MappingConverter
    {
        public static MappingModel ToMappingModel(Mapping mapping)
        {
            var request = (Request)mapping.RequestMatcher;
            var response = (Response)mapping.Provider;

            var clientIPMatchers = request.GetRequestMessageMatchers<RequestMessageClientIPMatcher>();
            var pathMatchers = request.GetRequestMessageMatchers<RequestMessagePathMatcher>();
            var urlMatchers = request.GetRequestMessageMatchers<RequestMessageUrlMatcher>();
            var headerMatchers = request.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
            var cookieMatchers = request.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
            var paramsMatchers = request.GetRequestMessageMatchers<RequestMessageParamMatcher>();
            var bodyMatcher = request.GetRequestMessageMatcher<RequestMessageBodyMatcher>();
            var methodMatcher = request.GetRequestMessageMatcher<RequestMessageMethodMatcher>();

            var mappingModel = new MappingModel
            {
                Guid = mapping.Guid,
                Title = mapping.Title,
                Priority = mapping.Priority,
                Scenario = mapping.Scenario,
                WhenStateIs = mapping.ExecutionConditionState,
                SetStateTo = mapping.NextState,
                Request = new RequestModel
                {
                    ClientIP = clientIPMatchers != null && clientIPMatchers.Any() ? new ClientIPModel
                    {
                        Matchers = Map(clientIPMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers)),
                        Funcs = Map(clientIPMatchers.Where(m => m.Funcs != null).SelectMany(m => m.Funcs))
                    } : null,

                    Path = pathMatchers != null && pathMatchers.Any() ? new PathModel
                    {
                        Matchers = Map(pathMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers)),
                        Funcs = Map(pathMatchers.Where(m => m.Funcs != null).SelectMany(m => m.Funcs))
                    } : null,

                    Url = urlMatchers != null && urlMatchers.Any() ? new UrlModel
                    {
                        Matchers = Map(urlMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers)),
                        Funcs = Map(urlMatchers.Where(m => m.Funcs != null).SelectMany(m => m.Funcs))
                    } : null,

                    Methods = methodMatcher?.Methods,

                    Headers = headerMatchers != null && headerMatchers.Any() ? headerMatchers.Select(hm => new HeaderModel
                    {
                        Name = hm.Name,
                        Matchers = Map(hm.Matchers),
                        Funcs = Map(hm.Funcs)
                    }).ToList() : null,

                    Cookies = cookieMatchers != null && cookieMatchers.Any() ? cookieMatchers.Select(cm => new CookieModel
                    {
                        Name = cm.Name,
                        Matchers = Map(cm.Matchers),
                        Funcs = Map(cm.Funcs)
                    }).ToList() : null,

                    Params = paramsMatchers != null && paramsMatchers.Any() ? paramsMatchers.Select(pm => new ParamModel
                    {
                        Name = pm.Key,
                        Values = pm.Values?.ToList(),
                        Funcs = Map(pm.Funcs)
                    }).ToList() : null,

                    Body = methodMatcher?.Methods != null && methodMatcher.Methods.Any(m => m == "get") ? null : new BodyModel
                    {
                        Matcher = bodyMatcher != null ? Map(bodyMatcher.Matcher) : null,
                        Func = bodyMatcher != null ? Map(bodyMatcher.Func) : null,
                        DataFunc = bodyMatcher != null ? Map(bodyMatcher.DataFunc) : null
                    }
                },
                Response = new ResponseModel
                {
                    Delay = response.Delay?.Milliseconds
                }
            };

            if (!string.IsNullOrEmpty(response.ProxyUrl))
            {
                mappingModel.Response.StatusCode = null;
                mappingModel.Response.Headers = null;
                mappingModel.Response.BodyDestination = null;
                mappingModel.Response.BodyAsJson = null;
                mappingModel.Response.Body = null;
                mappingModel.Response.BodyAsBytes = null;
                mappingModel.Response.BodyAsFile = null;
                mappingModel.Response.BodyAsFileIsCached = null;
                mappingModel.Response.UseTransformer = false;
                mappingModel.Response.BodyEncoding = null;
                mappingModel.Response.ProxyUrl = response.ProxyUrl;
            }
            else
            {
                mappingModel.Response.BodyDestination = response.ResponseMessage.BodyDestination;
                mappingModel.Response.StatusCode = response.ResponseMessage.StatusCode;
                mappingModel.Response.Headers = Map(response.ResponseMessage.Headers);
                mappingModel.Response.BodyAsJson = response.ResponseMessage.BodyAsJson;
                mappingModel.Response.Body = response.ResponseMessage.Body;
                mappingModel.Response.BodyAsBytes = response.ResponseMessage.BodyAsBytes;
                mappingModel.Response.BodyAsFile = response.ResponseMessage.BodyAsFile;
                mappingModel.Response.BodyAsFileIsCached = response.ResponseMessage.BodyAsFileIsCached;
                mappingModel.Response.UseTransformer = response.UseTransformer;

                if (response.ResponseMessage.BodyEncoding != null)
                {
                    mappingModel.Response.BodyEncoding = new EncodingModel
                    {
                        EncodingName = response.ResponseMessage.BodyEncoding.EncodingName,
                        CodePage = response.ResponseMessage.BodyEncoding.CodePage,
                        WebName = response.ResponseMessage.BodyEncoding.WebName
                    };
                }
            }

            return mappingModel;
        }

        private static IDictionary<string, object> Map(IDictionary<string, WireMockList<string>> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            var newDictionary = new Dictionary<string, object>();
            foreach (var entry in dictionary)
            {
                object value = entry.Value.Count == 1 ? (object)entry.Value.ToString() : entry.Value;
                newDictionary.Add(entry.Key, value);
            }

            return newDictionary;
        }

        private static MatcherModel[] Map([CanBeNull] IEnumerable<IMatcher> matchers)
        {
            if (matchers == null || !matchers.Any())
            {
                return null;
            }

            return matchers.Select(Map).Where(x => x != null).ToArray();
        }

        private static MatcherModel Map([CanBeNull] IMatcher matcher)
        {
            if (matcher == null)
            {
                return null;
            }

            IStringMatcher stringMatcher = matcher as IStringMatcher;
            string[] patterns = stringMatcher != null ? stringMatcher.GetPatterns() : new string[0];

            return new MatcherModel
            {
                Name = matcher.GetName(),
                Pattern = patterns.Length == 1 ? patterns.First() : null,
                Patterns = patterns.Length > 1 ? patterns : null
            };
        }

        private static string[] Map<T>([CanBeNull] IEnumerable<Func<T, bool>> funcs)
        {
            if (funcs == null || !funcs.Any())
                return null;

            return funcs.Select(Map).Where(x => x != null).ToArray();
        }

        private static string Map<T>([CanBeNull] Func<T, bool> func)
        {
            return func?.ToString();
        }

        public static IMatcher Map([CanBeNull] MatcherModel matcher)
        {
            if (matcher == null)
            {
                return null;
            }

            var parts = matcher.Name.Split('.');
            string matcherName = parts[0];
            string matcherType = parts.Length > 1 ? parts[1] : null;

            string[] patterns = matcher.Patterns ?? new[] { matcher.Pattern };

            switch (matcherName)
            {
                case "ExactMatcher":
                    return new ExactMatcher(patterns);

                case "RegexMatcher":
                    return new RegexMatcher(patterns);

                case "JsonPathMatcher":
                    return new JsonPathMatcher(patterns);

                case "XPathMatcher":
                    return new XPathMatcher(matcher.Pattern);

                case "WildcardMatcher":
                    return new WildcardMatcher(patterns, matcher.IgnoreCase == true);

                case "SimMetricsMatcher":
                    SimMetricType type = SimMetricType.Levenstein;
                    if (!string.IsNullOrEmpty(matcherType) && !Enum.TryParse(matcherType, out type))
                        throw new NotSupportedException($"Matcher '{matcherName}' with Type '{matcherType}' is not supported.");

                    return new SimMetricsMatcher(matcher.Pattern, type);

                default:
                    throw new NotSupportedException($"Matcher '{matcherName}' is not supported.");
            }
        }
    }
}
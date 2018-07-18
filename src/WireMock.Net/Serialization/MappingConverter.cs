using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
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
                        Matchers = MatcherMapper.Map(clientIPMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers))
                    } : null,

                    Path = pathMatchers != null && pathMatchers.Any() ? new PathModel
                    {
                        Matchers = MatcherMapper.Map(pathMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers))
                    } : null,

                    Url = urlMatchers != null && urlMatchers.Any() ? new UrlModel
                    {
                        Matchers = MatcherMapper.Map(urlMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers))
                    } : null,

                    Methods = methodMatcher?.Methods,

                    Headers = headerMatchers != null && headerMatchers.Any() ? headerMatchers.Select(hm => new HeaderModel
                    {
                        Name = hm.Name,
                        Matchers = MatcherMapper.Map(hm.Matchers)
                    }).ToList() : null,

                    Cookies = cookieMatchers != null && cookieMatchers.Any() ? cookieMatchers.Select(cm => new CookieModel
                    {
                        Name = cm.Name,
                        Matchers = MatcherMapper.Map(cm.Matchers)
                    }).ToList() : null,

                    Params = paramsMatchers != null && paramsMatchers.Any() ? paramsMatchers.Select(pm => new ParamModel
                    {
                        Name = pm.Key,
                        Matchers = MatcherMapper.Map(pm.Matchers)
                    }).ToList() : null,

                    Body = methodMatcher?.Methods != null && methodMatcher.Methods.Any(m => m == "get") ? null : new BodyModel
                    {
                        Matcher = bodyMatcher != null ? MatcherMapper.Map(bodyMatcher.Matcher) : null
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
                mappingModel.Response.BodyAsJsonIndented = null;
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
                mappingModel.Response.BodyAsJsonIndented = response.ResponseMessage.BodyAsJsonIndented;
                mappingModel.Response.Body = response.ResponseMessage.Body;
                mappingModel.Response.BodyAsBytes = response.ResponseMessage.BodyAsBytes;
                mappingModel.Response.BodyAsFile = response.ResponseMessage.BodyAsFile;
                mappingModel.Response.BodyAsFileIsCached = response.ResponseMessage.BodyAsFileIsCached;
                mappingModel.Response.UseTransformer = response.UseTransformer;

                if (response.ResponseMessage.BodyEncoding != null && response.ResponseMessage.BodyEncoding.WebName != "utf-8")
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
            if (dictionary == null || dictionary.Count == 0)
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
    }
}
using System.Collections.Generic;
using System.Linq;
using WireMock.Matchers.Request;
using WireMock.Models.Mappings;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Util;
using WireMock.Validation;
using WireMock.Types;

namespace WireMock.Serialization
{
    internal class MappingConverter
    {
        private readonly MatcherMapper _mapper;

        public MappingConverter(MatcherMapper mapper)
        {
            Check.NotNull(mapper, nameof(mapper));

            _mapper = mapper;
        }

        public MappingModel ToMappingModel(IMapping mapping)
        {
            var request = (Request)mapping.RequestMatcher;
            var response = (Response)mapping.Provider;

            var clientIPMatchers = request.GetRequestMessageMatchers<RequestMessageClientIPMatcher>();
            var pathMatchers = request.GetRequestMessageMatchers<RequestMessagePathMatcher>();
            var urlMatchers = request.GetRequestMessageMatchers<RequestMessageUrlMatcher>();
            var headerMatchers = request.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
            var cookieMatchers = request.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
            var paramsMatchers = request.GetRequestMessageMatchers<RequestMessageParamMatcher>();
            var methodMatcher = request.GetRequestMessageMatcher<RequestMessageMethodMatcher>();
            var bodyMatcher = request.GetRequestMessageMatcher<RequestMessageBodyMatcher>();

            var mappingModel = new MappingModel
            {
                Guid = mapping.Guid,
                Title = mapping.Title,
                Priority = mapping.Priority != 0 ? mapping.Priority : (int?)null,
                Scenario = mapping.Scenario,
                WhenStateIs = mapping.ExecutionConditionState,
                SetStateTo = mapping.NextState,
                Request = new RequestModel
                {
                    ClientIP = clientIPMatchers != null && clientIPMatchers.Any() ? new ClientIPModel
                    {
                        Matchers = _mapper.Map(clientIPMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers))
                    } : null,

                    Path = pathMatchers != null && pathMatchers.Any() ? new PathModel
                    {
                        Matchers = _mapper.Map(pathMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers))
                    } : null,

                    Url = urlMatchers != null && urlMatchers.Any() ? new UrlModel
                    {
                        Matchers = _mapper.Map(urlMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers))
                    } : null,

                    Methods = methodMatcher?.Methods,

                    Headers = headerMatchers != null && headerMatchers.Any() ? headerMatchers.Select(hm => new HeaderModel
                    {
                        Name = hm.Name,
                        Matchers = _mapper.Map(hm.Matchers)
                    }).ToList() : null,

                    Cookies = cookieMatchers != null && cookieMatchers.Any() ? cookieMatchers.Select(cm => new CookieModel
                    {
                        Name = cm.Name,
                        Matchers = _mapper.Map(cm.Matchers)
                    }).ToList() : null,

                    Params = paramsMatchers != null && paramsMatchers.Any() ? paramsMatchers.Select(pm => new ParamModel
                    {
                        Name = pm.Key,
                        IgnoreCase = pm.IgnoreCase == true ? true : (bool?)null,
                        Matchers = _mapper.Map(pm.Matchers)
                    }).ToList() : null
                },
                Response = new ResponseModel
                {
                    Delay = (int?)response.Delay?.TotalMilliseconds
                }
            };

            if (methodMatcher?.Methods != null && methodMatcher.Methods.All(m => m != "get") && bodyMatcher?.Matchers != null)
            {
                mappingModel.Request.Body = new BodyModel();

                if (bodyMatcher.Matchers.Length == 1)
                {
                    mappingModel.Request.Body.Matcher = _mapper.Map(bodyMatcher.Matchers[0]);
                }
                else if (bodyMatcher.Matchers.Length > 1)
                {
                    mappingModel.Request.Body.Matchers = _mapper.Map(bodyMatcher.Matchers);
                }
            }

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
                mappingModel.Response.UseTransformer = null;
                mappingModel.Response.BodyEncoding = null;
                mappingModel.Response.ProxyUrl = response.ProxyUrl;
                mappingModel.Response.Fault = null;
            }
            else
            {
                mappingModel.Response.BodyDestination = response.ResponseMessage.BodyDestination;
                mappingModel.Response.StatusCode = response.ResponseMessage.StatusCode;
                mappingModel.Response.Headers = Map(response.ResponseMessage.Headers);
                if (response.UseTransformer)
                {
                    mappingModel.Response.UseTransformer = response.UseTransformer;
                }

                if (response.ResponseMessage.BodyData != null)
                {
                    switch (response.ResponseMessage.BodyData?.DetectedBodyType)
                    {
                        case BodyType.String:
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

                    if (response.ResponseMessage.BodyData.Encoding != null && response.ResponseMessage.BodyData.Encoding.WebName != "utf-8")
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
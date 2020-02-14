﻿using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Validation;

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

            var clientIPMatchers = request.GetRequestMessageMatchers<RequestMessageClientIPMatcher>().Where(m => m.Matchers != null).SelectMany(m => m.Matchers).ToList();
            var pathMatchers = request.GetRequestMessageMatchers<RequestMessagePathMatcher>().Where(m => m.Matchers != null).SelectMany(m => m.Matchers).ToList();
            var urlMatchers = request.GetRequestMessageMatchers<RequestMessageUrlMatcher>().Where(m => m.Matchers != null).SelectMany(m => m.Matchers).ToList();
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
                    ClientIP = clientIPMatchers.Any() ? new ClientIPModel
                    {
                        Matchers = _mapper.Map(clientIPMatchers)
                    } : null,

                    Path = pathMatchers.Any() ? new PathModel
                    {
                        Matchers = _mapper.Map(pathMatchers)
                    } : null,

                    Url = urlMatchers.Any() ? new UrlModel
                    {
                        Matchers = _mapper.Map(urlMatchers)
                    } : null,

                    Methods = methodMatcher?.Methods,

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
                mappingModel.Response.UseTransformerForBodyAsFile = null;
                mappingModel.Response.BodyEncoding = null;
                mappingModel.Response.ProxyUrl = response.ProxyUrl;
                mappingModel.Response.Fault = null;
                mappingModel.Response.WebProxy = MapWebProxy(response.WebProxySettings);
            }
            else
            {
                mappingModel.Response.WebProxy = null;
                mappingModel.Response.BodyDestination = response.ResponseMessage.BodyDestination;
                mappingModel.Response.StatusCode = response.ResponseMessage.StatusCode;
                mappingModel.Response.Headers = MapHeaders(response.ResponseMessage.Headers);
                if (response.UseTransformer)
                {
                    mappingModel.Response.UseTransformer = response.UseTransformer;
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

        private static WebProxyModel MapWebProxy(IWebProxySettings settings)
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

            if (dictionary == null || dictionary.Count == 0)
            {
                return newDictionary;
            }

            foreach (var entry in dictionary)
            {
                object value = entry.Value.Count == 1 ? (object)entry.Value.ToString() : entry.Value;
                newDictionary.Add(entry.Key, value);
            }

            return newDictionary;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class FluentMockServer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        private void InitAdmin()
        {
            Given(Request.Create().WithUrl("/__admin/mappings").UsingGet()).RespondWith(new DynamicResponseProvider(MappingsGet));
            Given(Request.Create().WithUrl("/__admin/mappings").UsingPost()).RespondWith(new DynamicResponseProvider(MappingsPost));

            Given(Request.Create().WithUrl("/__admin/requests").UsingGet()).RespondWith(new DynamicResponseProvider(RequestsGet));
        }

        private ResponseMessage MappingsGet(RequestMessage requestMessage)
        {
            var result = new List<MappingModel>();
            foreach (var mapping in Mappings.Where(m => !(m.Provider is DynamicResponseProvider)))
            {
                var model = ToMappingModel(mapping);
                result.Add(model);
            }

            return ToJson(result);
        }

        private ResponseMessage MappingsPost(RequestMessage requestMessage)
        {
            var mappingModel = JsonConvert.DeserializeObject<MappingModel>(requestMessage.Body);

            if (mappingModel.Request == null)
                return new ResponseMessage { StatusCode = 400, Body = "Request missing" };

            if (mappingModel.Response == null)
                return new ResponseMessage { StatusCode = 400, Body = "Response missing" };

            var requestBuilder = InitRequestBuilder(mappingModel);
            var responseBuilder = InitResponseBuilder(mappingModel);

            IRespondWithAProviderGuid respondProvider = Given(requestBuilder);

            if (mappingModel.Guid != null && mappingModel.Guid != Guid.Empty)
                respondProvider = respondProvider.WithGuid(mappingModel.Guid.Value);

            respondProvider.RespondWith(responseBuilder);

            return new ResponseMessage { Body = "Mapping added" };
        }

        private IRequestBuilder InitRequestBuilder(MappingModel mappingModel)
        {
            IRequestBuilder requestBuilder = Request.Create();
            string url = mappingModel.Request.Url as string;
            if (url != null)
                requestBuilder = requestBuilder.WithUrl(url);
            else
                requestBuilder = requestBuilder.WithUrl("/*");
            //UrlModel urlModel = mappingModel.Request.Url as UrlModel;
            //if (urlModel?.Matchers != null)
            //    builder = builder.WithUrl(urlModel.Matchers.Select(Map).ToArray());

            if (mappingModel.Request.Methods != null)
                requestBuilder = requestBuilder.UsingVerb(mappingModel.Request.Methods);
            else
                requestBuilder = requestBuilder.UsingAnyVerb();

            if (mappingModel.Request.Headers != null)
            {
                foreach (var headerModel in mappingModel.Request.Headers.Where(h => h.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithHeader(headerModel.Name, headerModel.Matchers.Select(Map).ToArray());
                }
            }

            if (mappingModel.Request.Cookies != null)
            {
                foreach (var cookieModel in mappingModel.Request.Cookies.Where(c => c.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithCookie(cookieModel.Name, cookieModel.Matchers.Select(Map).ToArray());
                }
            }

            if (mappingModel.Request.Params != null)
            {
                foreach (var paramModel in mappingModel.Request.Params.Where(p => p.Values != null))
                {
                    requestBuilder = requestBuilder.WithParam(paramModel.Name, paramModel.Values.ToArray());
                }
            }

            if (mappingModel.Request.Body?.Matcher != null)
            {
                var bodyMatcher = Map(mappingModel.Request.Body.Matcher);
                requestBuilder = requestBuilder.WithBody(bodyMatcher);
            }
            return requestBuilder;
        }


        private IResponseBuilder InitResponseBuilder(MappingModel mappingModel)
        {
            IResponseBuilder responseBuilder = Response.Create();

            if (mappingModel.Response.StatusCode.HasValue)
                responseBuilder = responseBuilder.WithStatusCode(mappingModel.Response.StatusCode.Value);

            if (mappingModel.Response.Headers != null)
                responseBuilder = responseBuilder.WithHeaders(mappingModel.Response.Headers);

            if (mappingModel.Response.Body != null)
                responseBuilder = responseBuilder.WithBody(mappingModel.Response.Body);
            else if (mappingModel.Response.BodyAsJson != null)
                responseBuilder = responseBuilder.WithBodyAsJson(mappingModel.Response.BodyAsJson);
            else if (mappingModel.Response.BodyAsBase64 != null)
                responseBuilder = responseBuilder.WithBodyAsBase64(mappingModel.Response.BodyAsBase64);

            if (mappingModel.Response.UseTransformer)
                responseBuilder = responseBuilder.WithTransformer();
            return responseBuilder;
        }

        private ResponseMessage RequestsGet(RequestMessage requestMessage)
        {
            var result = new List<LogEntryModel>();
            foreach (var logEntry in LogEntries.Where(r => !r.RequestMessage.Path.StartsWith("/__admin/")))
            {
                var model = new LogEntryModel
                {
                    Request = new LogRequestModel
                    {
                        Guid = Guid.NewGuid(),
                        DateTime = logEntry.RequestMessage.DateTime,
                        Url = logEntry.RequestMessage.Path,
                        AbsoleteUrl = logEntry.RequestMessage.Url,
                        Query = logEntry.RequestMessage.Query,
                        Method = logEntry.RequestMessage.Method,
                        Body = logEntry.RequestMessage.Body,
                        Headers = logEntry.RequestMessage.Headers,
                        Cookies = logEntry.RequestMessage.Cookies
                    },
                    Response = new LogResponseModel
                    {
                        StatusCode = logEntry.ResponseMessage.StatusCode,
                        Body = logEntry.ResponseMessage.Body,
                        BodyOriginal = logEntry.ResponseMessage.BodyOriginal,
                        Headers = logEntry.ResponseMessage.Headers
                    }
                };

                result.Add(model);
            }

            return ToJson(result);
        }

        private MappingModel ToMappingModel(Mapping mapping)
        {
            var request = (Request)mapping.RequestMatcher;
            var response = (Response)mapping.Provider;

            var urlMatchers = request.GetRequestMessageMatchers<RequestMessageUrlMatcher>();
            var headerMatchers = request.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
            var cookieMatchers = request.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
            var paramsMatchers = request.GetRequestMessageMatchers<RequestMessageParamMatcher>();
            var bodyMatcher = request.GetRequestMessageMatcher<RequestMessageBodyMatcher>();
            var methodMatcher = request.GetRequestMessageMatcher<RequestMessageMethodMatcher>();

            return new MappingModel
            {
                Guid = mapping.Guid,
                Request = new RequestModel
                {
                    Url = new UrlModel
                    {
                        Matchers = urlMatchers != null ? Map(urlMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers)) : null
                    },
                    Methods = methodMatcher != null ? methodMatcher.Methods : new[] { "any" },
                    Headers = headerMatchers?.Select(hm => new HeaderModel
                    {
                        Name = hm.Name,
                        Matchers = Map(hm.Matchers)
                    }).ToList(),
                    Cookies = cookieMatchers?.Select(hm => new CookieModel
                    {
                        Name = hm.Name,
                        Matchers = Map(hm.Matchers)
                    }).ToList(),
                    Params = paramsMatchers?.Select(hm => new ParamModel
                    {
                        Name = hm.Key,
                        Values = hm.Values?.ToList()
                    }).ToList(),
                    Body = new BodyModel
                    {
                        Matcher = bodyMatcher != null ? Map(bodyMatcher.Matcher) : null
                    }
                },
                Response = new ResponseModel
                {
                    StatusCode = response.ResponseMessage.StatusCode,
                    Headers = response.ResponseMessage.Headers,
                    Body = response.ResponseMessage.Body,
                    UseTransformer = response.UseTransformer
                }
            };
        }

        private MatcherModel[] Map([CanBeNull] IEnumerable<IMatcher> matchers)
        {
            return matchers?.Select(Map).Where(x => x != null).ToArray();
        }

        private MatcherModel Map([CanBeNull] IMatcher matcher)
        {
            if (matcher == null)
                return null;

            return new MatcherModel
            {
                Name = matcher.GetType().Name,
                Pattern = matcher.GetPattern()
            };
        }

        private IMatcher Map([CanBeNull] MatcherModel matcher)
        {
            if (matcher == null)
                return null;

            switch (matcher.Name)
            {
                case "RegExMatcher":
                    return new RegexMatcher(matcher.Pattern);

                case "JsonPathMatcher":
                    return new JsonPathMatcher(matcher.Pattern);

                case "XPathMatcher":
                    return new XPathMatcher(matcher.Pattern);

                default:
                    return new WildcardMatcher(matcher.Pattern, matcher.IgnoreCase == true);
            }
        }

        private ResponseMessage ToJson<T>(T result)
        {
            return new ResponseMessage
            {
                Body = JsonConvert.SerializeObject(result, _settings),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
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
        private void InitAdmin()
        {
            Given(Request.Create().WithUrl("/__admin/mappings").UsingGet()).RespondWith(new DynamicResponseProvider(MappingsGet));

            Given(Request.Create().WithUrl("/__admin/requests").UsingGet()).RespondWith(new DynamicResponseProvider(RequestsGet));
        }

        private ResponseMessage RequestsGet()
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

        private ResponseMessage MappingsGet()
        {
            var result = new List<MappingModel>();
            foreach (var mapping in Mappings.Where(m => !(m.Provider is DynamicResponseProvider)))
            {
                var request = (Request) mapping.RequestMatcher;
                var urlMatchers = request.GetRequestMessageMatchers<RequestMessageUrlMatcher>();
                var headerMatchers = request.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
                var cookieMatchers = request.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
                var paramsMatchers = request.GetRequestMessageMatchers<RequestMessageParamMatcher>();
                var bodyMatcher = request.GetRequestMessageMatcher<RequestMessageBodyMatcher>();
                var methodMatcher = request.GetRequestMessageMatcher<RequestMessageMethodMatcher>();

                var response = (Response) mapping.Provider;

                var model = new MappingModel
                {
                    Guid = Guid.NewGuid(),
                    Request = new RequestModel
                    {
                        Url = new UrlModel
                        {
                            Matchers = urlMatchers != null ? Map(urlMatchers.Where(m => m.Matchers != null).SelectMany(m => m.Matchers)) : null
                        },
                        Methods = methodMatcher != null ? methodMatcher.Methods : new [] { "any" },
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
                        Body = response.ResponseMessage.Body
                    }
                };

                result.Add(model);
            }

            return ToJson(result);
        }

        private IList<MatcherModel> Map([CanBeNull] IEnumerable<IMatcher> matchers)
        {
            return matchers?.Select(Map).Where(x => x != null).ToList();
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

        private ResponseMessage ToJson<T>(T result)
        {
            return new ResponseMessage
            {
                Body = JsonConvert.SerializeObject(result, Formatting.Indented),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
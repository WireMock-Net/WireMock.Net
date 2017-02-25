using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SimMetrics.Net;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Admin.Settings;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class FluentMockServer
    {
        private const string AdminMappingsFolder = @"\__admin\mappings\";
        private const string AdminMappings = "/__admin/mappings";
        private const string AdminRequests = "/__admin/requests";
        private const string AdminSettings = "/__admin/settings";
        private readonly RegexMatcher _adminMappingsGuidPathMatcher = new RegexMatcher(@"^\/__admin\/mappings\/(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
        private readonly RegexMatcher _adminRequestsGuidPathMatcher = new RegexMatcher(@"^\/__admin\/requests\/(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        private void ReadStaticMappings()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + AdminMappingsFolder))
                return;

            foreach (string filename in Directory.EnumerateFiles(Directory.GetCurrentDirectory() + AdminMappingsFolder))
            {
                var json = File.ReadAllText(filename);
                DeserializeAndAddMapping(json, Guid.Parse(Path.GetFileNameWithoutExtension(filename)));
            }
        }

        private void InitAdmin()
        {
            // __admin/settings
            Given(Request.Create().WithPath(AdminSettings).UsingGet()).RespondWith(new DynamicResponseProvider(SettingsGet));
            Given(Request.Create().WithPath(AdminSettings).UsingVerb("PUT", "POST")).RespondWith(new DynamicResponseProvider(SettingsUpdate));


            // __admin/mappings
            Given(Request.Create().WithPath(AdminMappings).UsingGet()).RespondWith(new DynamicResponseProvider(MappingsGet));
            Given(Request.Create().WithPath(AdminMappings).UsingPost()).RespondWith(new DynamicResponseProvider(MappingsPost));
            Given(Request.Create().WithPath(AdminMappings).UsingDelete()).RespondWith(new DynamicResponseProvider(MappingsDelete));

            // __admin/mappings/reset
            Given(Request.Create().WithPath(AdminMappings + "/reset").UsingPost()).RespondWith(new DynamicResponseProvider(MappingsDelete));

            // __admin/mappings/{guid}
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingGet()).RespondWith(new DynamicResponseProvider(MappingGet));
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingPut().WithHeader("Content-Type", "application/json")).RespondWith(new DynamicResponseProvider(MappingPut));
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingDelete()).RespondWith(new DynamicResponseProvider(MappingDelete));

            // __admin/mappings/save
            Given(Request.Create().WithPath(AdminMappings + "/save").UsingPost()).RespondWith(new DynamicResponseProvider(MappingsSave));


            // __admin/requests
            Given(Request.Create().WithPath(AdminRequests).UsingGet()).RespondWith(new DynamicResponseProvider(RequestsGet));
            Given(Request.Create().WithPath(AdminRequests).UsingDelete()).RespondWith(new DynamicResponseProvider(RequestsDelete));

            // __admin/requests/reset
            Given(Request.Create().WithPath(AdminRequests + "/reset").UsingPost()).RespondWith(new DynamicResponseProvider(RequestsDelete));

            // __admin/request/{guid}
            Given(Request.Create().WithPath(_adminRequestsGuidPathMatcher).UsingGet()).RespondWith(new DynamicResponseProvider(RequestGet));
            Given(Request.Create().WithPath(_adminRequestsGuidPathMatcher).UsingDelete()).RespondWith(new DynamicResponseProvider(RequestDelete));

            // __admin/requests/find
            Given(Request.Create().WithPath(AdminRequests + "/find").UsingPost()).RespondWith(new DynamicResponseProvider(RequestsFind));
        }

        #region Settings
        private ResponseMessage SettingsGet(RequestMessage requestMessage)
        {
            var model = new SettingsModel
            {
                AllowPartialMapping = _allowPartialMapping,
                GlobalProcessingDelay = _requestProcessingDelay?.Milliseconds
            };

            return ToJson(model);
        }

        private ResponseMessage SettingsUpdate(RequestMessage requestMessage)
        {
            var settings = JsonConvert.DeserializeObject<SettingsModel>(requestMessage.Body);

            if (settings.AllowPartialMapping != null)
                _allowPartialMapping = settings.AllowPartialMapping.Value;

            if (settings.GlobalProcessingDelay != null)
                _requestProcessingDelay = TimeSpan.FromMilliseconds(settings.GlobalProcessingDelay.Value);

            return new ResponseMessage { Body = "Settings updated" };
        }
        #endregion Settings

        #region Mapping/{guid}
        private ResponseMessage MappingGet(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminMappings.Length + 1));
            var mapping = Mappings.FirstOrDefault(m => !m.IsAdminInterface && m.Guid == guid);

            if (mapping == null)
                return new ResponseMessage { StatusCode = 404, Body = "Mapping not found" };

            var model = ToMappingModel(mapping);

            return ToJson(model);
        }

        private ResponseMessage MappingPut(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.TrimStart(AdminMappings.ToCharArray()));
            var mappingModel = JsonConvert.DeserializeObject<MappingModel>(requestMessage.Body);

            if (mappingModel.Request == null)
                return new ResponseMessage { StatusCode = 400, Body = "Request missing" };

            if (mappingModel.Response == null)
                return new ResponseMessage { StatusCode = 400, Body = "Response missing" };

            var requestBuilder = InitRequestBuilder(mappingModel.Request);
            var responseBuilder = InitResponseBuilder(mappingModel.Response);

            Given(requestBuilder)
                .WithGuid(guid)
                .RespondWith(responseBuilder);

            return new ResponseMessage { Body = "Mapping added or updated" };
        }

        private ResponseMessage MappingDelete(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminMappings.Length + 1));

            if (DeleteMapping(guid))
                return new ResponseMessage { Body = "Mapping removed" };

            return new ResponseMessage { Body = "Mapping not found" };
        }
        #endregion Mapping/{guid}

        #region Mappings
        private ResponseMessage MappingsSave(RequestMessage requestMessage)
        {
            string folder = Directory.GetCurrentDirectory() + AdminMappingsFolder;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var mapping in Mappings.Where(m => !m.IsAdminInterface))
            {
                var model = ToMappingModel(mapping);
                string json = JsonConvert.SerializeObject(model, _settings);

                File.WriteAllText(Path.Combine(folder, mapping.Guid + ".json"), json);
            }

            return new ResponseMessage { Body = "Mappings saved to disk" };
        }

        private ResponseMessage MappingsGet(RequestMessage requestMessage)
        {
            var result = new List<MappingModel>();
            foreach (var mapping in Mappings.Where(m => !m.IsAdminInterface))
            {
                var model = ToMappingModel(mapping);
                result.Add(model);
            }

            return ToJson(result);
        }

        private ResponseMessage MappingsPost(RequestMessage requestMessage)
        {
            try
            {
                DeserializeAndAddMapping(requestMessage.Body);
            }
            catch (ArgumentException a)
            {
                return new ResponseMessage { StatusCode = 400, Body = a.Message };
            }
            catch (Exception e)
            {
                return new ResponseMessage { StatusCode = 500, Body = e.ToString() };
            }

            return new ResponseMessage { StatusCode = 201, Body = "Mapping added" };
        }

        private void DeserializeAndAddMapping(string json, Guid? guid = null)
        {
            var mappingModel = JsonConvert.DeserializeObject<MappingModel>(json);

            Check.NotNull(mappingModel, nameof(mappingModel));
            Check.NotNull(mappingModel.Request, nameof(mappingModel.Request));
            Check.NotNull(mappingModel.Response, nameof(mappingModel.Response));

            var requestBuilder = InitRequestBuilder(mappingModel.Request);
            var responseBuilder = InitResponseBuilder(mappingModel.Response);

            IRespondWithAProvider respondProvider = Given(requestBuilder);

            if (guid != null)
            {
                respondProvider = respondProvider.WithGuid(guid.Value);
            }
            else if (mappingModel.Guid != null && mappingModel.Guid != Guid.Empty)
            {
                respondProvider = respondProvider.WithGuid(mappingModel.Guid.Value);
            }

            if (mappingModel.Priority != null)
                respondProvider = respondProvider.AtPriority(mappingModel.Priority.Value);

            respondProvider.RespondWith(responseBuilder);
        }

        private ResponseMessage MappingsDelete(RequestMessage requestMessage)
        {
            ResetMappings();

            return new ResponseMessage { Body = "Mappings deleted" };
        }
        #endregion Mappings

        #region Request/{guid}
        private ResponseMessage RequestGet(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminRequests.Length + 1));
            var entry = LogEntries.FirstOrDefault(r => !r.RequestMessage.Path.StartsWith("/__admin/") && r.Guid == guid);

            if (entry == null)
                return new ResponseMessage { StatusCode = 404, Body = "Request not found" };

            var model = ToLogEntryModel(entry);

            return ToJson(model);
        }

        private ResponseMessage RequestDelete(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminRequests.Length + 1));

            if (DeleteLogEntry(guid))
                return new ResponseMessage { Body = "Request removed" };

            return new ResponseMessage { Body = "Request not found" };
        }
        #endregion Request/{guid}

        #region Requests
        private ResponseMessage RequestsGet(RequestMessage requestMessage)
        {
            var result = LogEntries
                .Where(r => !r.RequestMessage.Path.StartsWith("/__admin/"))
                .Select(ToLogEntryModel);

            return ToJson(result);
        }

        private LogEntryModel ToLogEntryModel(LogEntry logEntry)
        {
            return new LogEntryModel
            {
                Guid = logEntry.Guid,
                Request = new LogRequestModel
                {
                    DateTime = logEntry.RequestMessage.DateTime,
                    Path = logEntry.RequestMessage.Path,
                    AbsoleteUrl = logEntry.RequestMessage.Url,
                    Query = logEntry.RequestMessage.Query,
                    Method = logEntry.RequestMessage.Method,
                    Body = logEntry.RequestMessage.Body,
                    Headers = logEntry.RequestMessage.Headers,
                    Cookies = logEntry.RequestMessage.Cookies,
                    BodyEncoding = logEntry.RequestMessage.BodyEncoding != null ? new EncodingModel
                    {
                        EncodingName = logEntry.RequestMessage.BodyEncoding.EncodingName,
                        CodePage = logEntry.RequestMessage.BodyEncoding.CodePage,
                        WebName = logEntry.RequestMessage.BodyEncoding.WebName
                    } : null
                },
                Response = new LogResponseModel
                {
                    StatusCode = logEntry.ResponseMessage.StatusCode,
                    Body = logEntry.ResponseMessage.Body,
                    BodyOriginal = logEntry.ResponseMessage.BodyOriginal,
                    Headers = logEntry.ResponseMessage.Headers,
                    BodyEncoding = logEntry.ResponseMessage.BodyEncoding != null ? new EncodingModel
                    {
                        EncodingName = logEntry.ResponseMessage.BodyEncoding.EncodingName,
                        CodePage = logEntry.ResponseMessage.BodyEncoding.CodePage,
                        WebName = logEntry.ResponseMessage.BodyEncoding.WebName
                    } : null
                },
                MappingGuid = logEntry.MappingGuid,
                RequestMatchResult = logEntry.RequestMatchResult != null ? new LogRequestMatchModel
                {
                    TotalScore = logEntry.RequestMatchResult.TotalScore,
                    TotalNumber = logEntry.RequestMatchResult.TotalNumber,
                    IsPerfectMatch = logEntry.RequestMatchResult.IsPerfectMatch,
                    AverageTotalScore = logEntry.RequestMatchResult.AverageTotalScore
                } : null
            };
        }

        private ResponseMessage RequestsDelete(RequestMessage requestMessage)
        {
            ResetLogEntries();

            return new ResponseMessage { Body = "Requests deleted" };
        }
        #endregion Requests

        #region Requests/find
        private ResponseMessage RequestsFind(RequestMessage requestMessage)
        {
            var requestModel = JsonConvert.DeserializeObject<RequestModel>(requestMessage.Body);

            var request = (Request)InitRequestBuilder(requestModel);

            var dict = new Dictionary<LogEntry, RequestMatchResult>();
            foreach (var logEntry in LogEntries.Where(le => !le.RequestMessage.Path.StartsWith("/__admin/")))
            {
                var requestMatchResult = new RequestMatchResult();
                if (request.GetMatchingScore(logEntry.RequestMessage, requestMatchResult) > 0.99)
                    dict.Add(logEntry, requestMatchResult);
            }

            var result = dict.OrderBy(x => x.Value.AverageTotalScore).Select(x => x.Key);

            return ToJson(result);
        }
        #endregion Requests/find

        private IRequestBuilder InitRequestBuilder(RequestModel requestModel)
        {
            IRequestBuilder requestBuilder = Request.Create();

            if (requestModel.Path != null)
            {
                string path = requestModel.Path as string;
                if (path != null)
                    requestBuilder = requestBuilder.WithPath(path);
                else
                {
                    var pathModel = JsonUtils.ParseJTokenToObject<PathModel>(requestModel.Path);
                    if (pathModel?.Matchers != null)
                        requestBuilder = requestBuilder.WithPath(pathModel.Matchers.Select(Map).ToArray());
                }
            }

            if (requestModel.Url != null)
            {
                string url = requestModel.Url as string;
                if (url != null)
                    requestBuilder = requestBuilder.WithUrl(url);
                else
                {
                    var urlModel = JsonUtils.ParseJTokenToObject<UrlModel>(requestModel.Url);
                    if (urlModel?.Matchers != null)
                        requestBuilder = requestBuilder.WithUrl(urlModel.Matchers.Select(Map).ToArray());
                }
            }

            if (requestModel.Methods != null)
                requestBuilder = requestBuilder.UsingVerb(requestModel.Methods);

            if (requestModel.Headers != null)
            {
                foreach (var headerModel in requestModel.Headers.Where(h => h.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithHeader(headerModel.Name, headerModel.Matchers.Select(Map).ToArray());
                }
            }

            if (requestModel.Cookies != null)
            {
                foreach (var cookieModel in requestModel.Cookies.Where(c => c.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithCookie(cookieModel.Name, cookieModel.Matchers.Select(Map).ToArray());
                }
            }

            if (requestModel.Params != null)
            {
                foreach (var paramModel in requestModel.Params.Where(p => p.Values != null))
                {
                    requestBuilder = requestBuilder.WithParam(paramModel.Name, paramModel.Values.ToArray());
                }
            }

            if (requestModel.Body?.Matcher != null)
            {
                var bodyMatcher = Map(requestModel.Body.Matcher);
                requestBuilder = requestBuilder.WithBody(bodyMatcher);
            }

            return requestBuilder;
        }

        private IResponseBuilder InitResponseBuilder(ResponseModel responseModel)
        {
            IResponseBuilder responseBuilder = Response.Create();

            if (responseModel.StatusCode.HasValue)
                responseBuilder = responseBuilder.WithStatusCode(responseModel.StatusCode.Value);

            if (responseModel.Headers != null)
                responseBuilder = responseBuilder.WithHeaders(responseModel.Headers);

            if (responseModel.Body != null)
                responseBuilder = responseBuilder.WithBody(responseModel.Body, ToEncoding(responseModel.BodyEncoding));
            else if (responseModel.BodyAsJson != null)
                responseBuilder = responseBuilder.WithBodyAsJson(responseModel.BodyAsJson, ToEncoding(responseModel.BodyEncoding));
            else if (responseModel.BodyAsBase64 != null)
                responseBuilder = responseBuilder.WithBodyAsBase64(responseModel.BodyAsBase64, ToEncoding(responseModel.BodyEncoding));

            if (responseModel.UseTransformer)
                responseBuilder = responseBuilder.WithTransformer();

            if (responseModel.Delay > 0)
                responseBuilder = responseBuilder.WithDelay(responseModel.Delay.Value);

            return responseBuilder;
        }

        private MappingModel ToMappingModel(Mapping mapping)
        {
            var request = (Request)mapping.RequestMatcher;
            var response = (Response)mapping.Provider;

            var pathMatchers = request.GetRequestMessageMatchers<RequestMessagePathMatcher>();
            var urlMatchers = request.GetRequestMessageMatchers<RequestMessageUrlMatcher>();
            var headerMatchers = request.GetRequestMessageMatchers<RequestMessageHeaderMatcher>();
            var cookieMatchers = request.GetRequestMessageMatchers<RequestMessageCookieMatcher>();
            var paramsMatchers = request.GetRequestMessageMatchers<RequestMessageParamMatcher>();
            var bodyMatcher = request.GetRequestMessageMatcher<RequestMessageBodyMatcher>();
            var methodMatcher = request.GetRequestMessageMatcher<RequestMessageMethodMatcher>();

            return new MappingModel
            {
                Guid = mapping.Guid,
                Priority = mapping.Priority,
                Request = new RequestModel
                {
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

                    Params = paramsMatchers != null && paramsMatchers.Any() ? paramsMatchers?.Select(pm => new ParamModel
                    {
                        Name = pm.Key,
                        Values = pm.Values?.ToList(),
                        Funcs = Map(pm.Funcs)
                    }).ToList() : null,

                    Body = new BodyModel
                    {
                        Matcher = bodyMatcher != null ? Map(bodyMatcher.Matcher) : null,
                        Func = bodyMatcher != null ? Map(bodyMatcher.Func) : null,
                        DataFunc = bodyMatcher != null ? Map(bodyMatcher.DataFunc) : null
                    }
                },
                Response = new ResponseModel
                {
                    StatusCode = response.ResponseMessage.StatusCode,
                    Headers = response.ResponseMessage.Headers,
                    Body = response.ResponseMessage.Body,
                    UseTransformer = response.UseTransformer,
                    Delay = response.Delay?.Milliseconds,

                    BodyEncoding = response.ResponseMessage.BodyEncoding != null ? new EncodingModel
                    {
                        EncodingName = response.ResponseMessage.BodyEncoding.EncodingName,
                        CodePage = response.ResponseMessage.BodyEncoding.CodePage,
                        WebName = response.ResponseMessage.BodyEncoding.WebName
                    } : null
                }
            };
        }

        private MatcherModel[] Map([CanBeNull] IEnumerable<IMatcher> matchers)
        {
            if (matchers == null || !matchers.Any())
                return null;

            return matchers.Select(Map).Where(x => x != null).ToArray();
        }

        private MatcherModel Map([CanBeNull] IMatcher matcher)
        {
            if (matcher == null)
                return null;

            var patterns = matcher.GetPatterns();

            return new MatcherModel
            {
                Name = matcher.GetName(),
                Pattern = patterns.Length == 1 ? patterns.First() : null,
                Patterns = patterns.Length > 1 ? patterns : null
            };
        }

        private string[] Map<T>([CanBeNull] IEnumerable<Func<T, bool>> funcs)
        {
            if (funcs == null || !funcs.Any())
                return null;

            return funcs.Select(Map).Where(x => x != null).ToArray();
        }

        private string Map<T>([CanBeNull] Func<T, bool> func)
        {
            return func?.ToString();
        }

        private IMatcher Map([CanBeNull] MatcherModel matcher)
        {
            if (matcher == null)
                return null;

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

        private ResponseMessage ToJson<T>(T result)
        {
            return new ResponseMessage
            {
                Body = JsonConvert.SerializeObject(result, _settings),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        private Encoding ToEncoding(EncodingModel encodingModel)
        {
            return encodingModel != null ? Encoding.GetEncoding(encodingModel.CodePage) : null;
        }
    }
}
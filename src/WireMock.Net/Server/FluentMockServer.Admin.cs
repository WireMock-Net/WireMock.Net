using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Admin.Settings;
using WireMock.Http;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class FluentMockServer
    {
        private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");
        private const string AdminMappings = "/__admin/mappings";
        private const string AdminRequests = "/__admin/requests";
        private const string AdminSettings = "/__admin/settings";
        private const string AdminScenarios = "/__admin/scenarios";
        private readonly RegexMatcher _adminMappingsGuidPathMatcher = new RegexMatcher(@"^\/__admin\/mappings\/(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
        private readonly RegexMatcher _adminRequestsGuidPathMatcher = new RegexMatcher(@"^\/__admin\/requests\/(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        /// <summary>
        /// Reads the static mappings from a folder.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use \__admin\mappings\</param>
        [PublicAPI]
        public void ReadStaticMappings([CanBeNull] string folder = null)
        {
            if (folder == null)
                folder = Path.Combine(Directory.GetCurrentDirectory(), AdminMappingsFolder);

            if (!Directory.Exists(folder))
                return;

            foreach (string filename in Directory.EnumerateFiles(folder).OrderBy(f => f))
            {
                ReadStaticMapping(filename);
            }
        }

        /// <summary>
        /// Reads the static mapping.
        /// </summary>
        /// <param name="filename">The filename.</param>
        [PublicAPI]
        public void ReadStaticMapping([NotNull] string filename)
        {
            Check.NotNull(filename, nameof(filename));

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

            if (Guid.TryParse(filenameWithoutExtension, out var guidFromFilename))
            {
                DeserializeAndAddMapping(File.ReadAllText(filename), guidFromFilename);
            }
            else
            {
                DeserializeAndAddMapping(File.ReadAllText(filename));
            }
        }

        private void InitAdmin()
        {
            // __admin/settings
            Given(Request.Create().WithPath(AdminSettings).UsingGet()).RespondWith(new DynamicResponseProvider(SettingsGet));
            Given(Request.Create().WithPath(AdminSettings).UsingVerb("PUT", "POST").WithHeader("Content-Type", "application/json")).RespondWith(new DynamicResponseProvider(SettingsUpdate));


            // __admin/mappings
            Given(Request.Create().WithPath(AdminMappings).UsingGet()).RespondWith(new DynamicResponseProvider(MappingsGet));
            Given(Request.Create().WithPath(AdminMappings).UsingPost().WithHeader("Content-Type", "application/json")).RespondWith(new DynamicResponseProvider(MappingsPost));
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


            // __admin/scenarios
            Given(Request.Create().WithPath(AdminScenarios).UsingGet()).RespondWith(new DynamicResponseProvider(ScenariosGet));
            Given(Request.Create().WithPath(AdminScenarios).UsingDelete()).RespondWith(new DynamicResponseProvider(ScenariosReset));

            // __admin/scenarios/reset
            Given(Request.Create().WithPath(AdminScenarios + "/reset").UsingPost()).RespondWith(new DynamicResponseProvider(ScenariosReset));
        }

        #region Proxy and Record
        private HttpClient _httpClientForProxy;

        private void InitProxyAndRecord(IProxyAndRecordSettings settings)
        {
            _httpClientForProxy = HttpClientHelper.CreateHttpClient(settings.X509Certificate2ThumbprintOrSubjectName);
            Given(Request.Create().WithPath("/*").UsingAnyVerb()).RespondWith(new ProxyAsyncResponseProvider(ProxyAndRecordAsync, settings));
        }

        private async Task<ResponseMessage> ProxyAndRecordAsync(RequestMessage requestMessage, IProxyAndRecordSettings settings)
        {
            var requestUri = new Uri(requestMessage.Url);
            var proxyUri = new Uri(settings.Url);
            var proxyUriWithRequestPathAndQuery = new Uri(proxyUri, requestUri.PathAndQuery);

            var responseMessage = await HttpClientHelper.SendAsync(_httpClientForProxy, requestMessage, proxyUriWithRequestPathAndQuery.AbsoluteUri);

            if (settings.SaveMapping)
            {
                var mapping = ToMapping(requestMessage, responseMessage);
                _options.Mappings.Add(mapping);

                if (settings.SaveMappingToFile)
                {
                    SaveMappingToFile(mapping);
                }
            }

            return responseMessage;
        }

        private Mapping ToMapping(RequestMessage requestMessage, ResponseMessage responseMessage)
        {
            var request = Request.Create();
            request.WithPath(requestMessage.Path);
            request.UsingVerb(requestMessage.Method);

            requestMessage.Query.Loop((key, value) => request.WithParam(key, value.ToArray()));
            requestMessage.Headers.Loop((key, value) => request.WithHeader(key, value.ToArray()));
            requestMessage.Cookies.Loop((key, value) => request.WithCookie(key, value));

            if (requestMessage.Body != null)
            {
                request.WithBody(new ExactMatcher(requestMessage.Body));
            }

            var response = Response.Create(responseMessage);

            return new Mapping(Guid.NewGuid(), string.Empty, request, response, 0, null, null, null);
        }
        #endregion

        #region Settings
        private ResponseMessage SettingsGet(RequestMessage requestMessage)
        {
            var model = new SettingsModel
            {
                AllowPartialMapping = _options.AllowPartialMapping,
                MaxRequestLogCount = _options.MaxRequestLogCount,
                RequestLogExpirationDuration = _options.RequestLogExpirationDuration,
                GlobalProcessingDelay = (int?)_options.RequestProcessingDelay?.TotalMilliseconds
            };

            return ToJson(model);
        }

        private ResponseMessage SettingsUpdate(RequestMessage requestMessage)
        {
            var settings = JsonConvert.DeserializeObject<SettingsModel>(requestMessage.Body);

            if (settings.AllowPartialMapping != null)
                _options.AllowPartialMapping = settings.AllowPartialMapping.Value;

            _options.MaxRequestLogCount = settings.MaxRequestLogCount;

            _options.RequestLogExpirationDuration = settings.RequestLogExpirationDuration;

            if (settings.GlobalProcessingDelay != null)
                _options.RequestProcessingDelay = TimeSpan.FromMilliseconds(settings.GlobalProcessingDelay.Value);

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

            var model = MappingConverter.ToMappingModel(mapping);

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

            IRespondWithAProvider respondProvider = Given(requestBuilder).WithGuid(guid);

            if (!string.IsNullOrEmpty(mappingModel.Title))
                respondProvider = respondProvider.WithTitle(mappingModel.Title);

            respondProvider.RespondWith(responseBuilder);

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
            foreach (var mapping in Mappings.Where(m => !m.IsAdminInterface))
            {
                SaveMappingToFile(mapping);
            }

            return new ResponseMessage { Body = "Mappings saved to disk" };
        }

        private void SaveMappingToFile(Mapping mapping)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), AdminMappingsFolder);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var model = MappingConverter.ToMappingModel(mapping);
            string json = JsonConvert.SerializeObject(model, _settings);
            string filename = !string.IsNullOrEmpty(mapping.Title) ? SanitizeFileName(mapping.Title) : mapping.Guid.ToString();

            File.WriteAllText(Path.Combine(folder, filename + ".json"), json);
        }

        private static string SanitizeFileName(string name, char replaceChar = '_')
        {
            return Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, replaceChar));
        }

        private ResponseMessage MappingsGet(RequestMessage requestMessage)
        {
            var result = new List<MappingModel>();
            foreach (var mapping in Mappings.Where(m => !m.IsAdminInterface))
            {
                var model = MappingConverter.ToMappingModel(mapping);
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

            if (!string.IsNullOrEmpty(mappingModel.Title))
                respondProvider = respondProvider.WithTitle(mappingModel.Title);

            if (mappingModel.Priority != null)
                respondProvider = respondProvider.AtPriority(mappingModel.Priority.Value);

            if (mappingModel.Scenario != null)
            {
                respondProvider = respondProvider.InScenario(mappingModel.Scenario);
                respondProvider = respondProvider.WhenStateIs(mappingModel.WhenStateIs);
                respondProvider = respondProvider.WillSetStateTo(mappingModel.SetStateTo);
            }

            respondProvider.RespondWith(responseBuilder);
        }

        private ResponseMessage MappingsDelete(RequestMessage requestMessage)
        {
            ResetMappings();

            ResetScenarios();

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
                    ClientIP = logEntry.RequestMessage.ClientIP,
                    Path = logEntry.RequestMessage.Path,
                    AbsoluteUrl = logEntry.RequestMessage.Url,
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
                    BodyDestination = logEntry.ResponseMessage.BodyDestination,
                    Body = logEntry.ResponseMessage.Body,
                    BodyAsBytes = logEntry.ResponseMessage.BodyAsBytes,
                    BodyOriginal = logEntry.ResponseMessage.BodyOriginal,
                    BodyAsFile = logEntry.ResponseMessage.BodyAsFile,
                    BodyAsFileIsCached = logEntry.ResponseMessage.BodyAsFileIsCached,
                    Headers = logEntry.ResponseMessage.Headers,
                    BodyEncoding = logEntry.ResponseMessage.BodyEncoding != null ? new EncodingModel
                    {
                        EncodingName = logEntry.ResponseMessage.BodyEncoding.EncodingName,
                        CodePage = logEntry.ResponseMessage.BodyEncoding.CodePage,
                        WebName = logEntry.ResponseMessage.BodyEncoding.WebName
                    } : null
                },
                MappingGuid = logEntry.MappingGuid,
                MappingTitle = logEntry.MappingTitle,
                RequestMatchResult = logEntry.RequestMatchResult != null ? new LogRequestMatchModel
                {
                    TotalScore = logEntry.RequestMatchResult.TotalScore,
                    TotalNumber = logEntry.RequestMatchResult.TotalNumber,
                    IsPerfectMatch = logEntry.RequestMatchResult.IsPerfectMatch,
                    AverageTotalScore = logEntry.RequestMatchResult.AverageTotalScore,
                    MatchDetails = logEntry.RequestMatchResult.MatchDetails.Select(x => new
                    {
                        Name = x.Key.Name.Replace("RequestMessage", string.Empty),
                        Score = x.Value
                    } as object).ToList()
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
                if (request.GetMatchingScore(logEntry.RequestMessage, requestMatchResult) > MatchScores.AlmostPerfect)
                {
                    dict.Add(logEntry, requestMatchResult);
                }
            }

            var result = dict.OrderBy(x => x.Value.AverageTotalScore).Select(x => x.Key).Select(ToLogEntryModel);

            return ToJson(result);
        }
        #endregion Requests/find

        #region Scenarios
        private ResponseMessage ScenariosGet(RequestMessage requestMessage)
        {
            var scenarios = Scenarios.Select(s => new
            {
                Name = s.Key,
                Started = s.Value != null,
                NextState = s.Value
            });
            return ToJson(scenarios);
        }

        private ResponseMessage ScenariosReset(RequestMessage requestMessage)
        {
            ResetScenarios();

            return new ResponseMessage { Body = "Scenarios reset" };
        }
        #endregion

        private IRequestBuilder InitRequestBuilder(RequestModel requestModel)
        {
            IRequestBuilder requestBuilder = Request.Create();

            if (requestModel.ClientIP != null)
            {
                string clientIP = requestModel.ClientIP as string;
                if (clientIP != null)
                {
                    requestBuilder = requestBuilder.WithClientIP(clientIP);
                }
                else
                {
                    var clientIPModel = JsonUtils.ParseJTokenToObject<ClientIPModel>(requestModel.ClientIP);
                    if (clientIPModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithPath(clientIPModel.Matchers.Select(MappingConverter.Map).ToArray());
                    }
                }
            }

            if (requestModel.Path != null)
            {
                string path = requestModel.Path as string;
                if (path != null)
                {
                    requestBuilder = requestBuilder.WithPath(path);
                }
                else
                {
                    var pathModel = JsonUtils.ParseJTokenToObject<PathModel>(requestModel.Path);
                    if (pathModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithPath(pathModel.Matchers.Select(MappingConverter.Map).ToArray());
                    }
                }
            }

            if (requestModel.Url != null)
            {
                string url = requestModel.Url as string;
                if (url != null)
                {
                    requestBuilder = requestBuilder.WithUrl(url);
                }
                else
                {
                    var urlModel = JsonUtils.ParseJTokenToObject<UrlModel>(requestModel.Url);
                    if (urlModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithUrl(urlModel.Matchers.Select(MappingConverter.Map).ToArray());
                    }
                }
            }

            if (requestModel.Methods != null)
            {
                requestBuilder = requestBuilder.UsingVerb(requestModel.Methods);
            }

            if (requestModel.Headers != null)
            {
                foreach (var headerModel in requestModel.Headers.Where(h => h.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithHeader(headerModel.Name, headerModel.Matchers.Select(MappingConverter.Map).ToArray());
                }
            }

            if (requestModel.Cookies != null)
            {
                foreach (var cookieModel in requestModel.Cookies.Where(c => c.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithCookie(cookieModel.Name, cookieModel.Matchers.Select(MappingConverter.Map).ToArray());
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
                var bodyMatcher = MappingConverter.Map(requestModel.Body.Matcher);
                requestBuilder = requestBuilder.WithBody(bodyMatcher);
            }

            return requestBuilder;
        }

        private IResponseBuilder InitResponseBuilder(ResponseModel responseModel)
        {
            IResponseBuilder responseBuilder = Response.Create();

            if (responseModel.Delay > 0)
            {
                responseBuilder = responseBuilder.WithDelay(responseModel.Delay.Value);
            }

            if (!string.IsNullOrEmpty(responseModel.ProxyUrl))
            {
                if (string.IsNullOrEmpty(responseModel.X509Certificate2ThumbprintOrSubjectName))
                {
                    return responseBuilder.WithProxy(responseModel.ProxyUrl);
                }

                return responseBuilder.WithProxy(responseModel.ProxyUrl, responseModel.X509Certificate2ThumbprintOrSubjectName);
            }

            if (responseModel.StatusCode.HasValue)
            {
                responseBuilder = responseBuilder.WithStatusCode(responseModel.StatusCode.Value);
            }

            if (responseModel.Headers != null)
            {
                foreach (var entry in responseModel.Headers)
                {
                    responseBuilder = entry.Value is string value ?
                        responseBuilder.WithHeader(entry.Key, value) :
                        responseBuilder.WithHeader(entry.Key, JsonUtils.ParseJTokenToObject<string[]>(entry.Value));
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
                responseBuilder = responseBuilder.WithBodyAsJson(responseModel.BodyAsJson, ToEncoding(responseModel.BodyEncoding));
            }
            else if (responseModel.BodyFromBase64 != null)
            {
                responseBuilder = responseBuilder.WithBodyFromBase64(responseModel.BodyFromBase64, ToEncoding(responseModel.BodyEncoding));
            }

            if (responseModel.UseTransformer)
            {
                responseBuilder = responseBuilder.WithTransformer();
            }

            return responseBuilder;
        }

        private ResponseMessage ToJson<T>(T result)
        {
            return new ResponseMessage
            {
                Body = JsonConvert.SerializeObject(result, _settings),
                StatusCode = 200,
                Headers = new Dictionary<string, WireMockList<string>> { { "Content-Type", new WireMockList<string>("application/json") } }
            };
        }

        private Encoding ToEncoding(EncodingModel encodingModel)
        {
            return encodingModel != null ? Encoding.GetEncoding(encodingModel.CodePage) : null;
        }
    }
}
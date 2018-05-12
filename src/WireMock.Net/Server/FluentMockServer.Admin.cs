using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Admin.Settings;
using WireMock.Http;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
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
        private const string ContentTypeJson = "application/json";
        private const string AdminMappings = "/__admin/mappings";
        private const string AdminRequests = "/__admin/requests";
        private const string AdminSettings = "/__admin/settings";
        private const string AdminScenarios = "/__admin/scenarios";
        private readonly RegexMatcher _adminMappingsGuidPathMatcher = new RegexMatcher(MatchBehaviour.AcceptOnMatch, @"^\/__admin\/mappings\/(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
        private readonly RegexMatcher _adminRequestsGuidPathMatcher = new RegexMatcher(MatchBehaviour.AcceptOnMatch, @"^\/__admin\/requests\/(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        #region InitAdmin
        private void InitAdmin()
        {
            // __admin/settings
            Given(Request.Create().WithPath(AdminSettings).UsingGet()).RespondWith(new DynamicResponseProvider(SettingsGet));
            Given(Request.Create().WithPath(AdminSettings).UsingMethod("PUT", "POST").WithHeader(HttpKnownHeaderNames.ContentType, ContentTypeJson)).RespondWith(new DynamicResponseProvider(SettingsUpdate));


            // __admin/mappings
            Given(Request.Create().WithPath(AdminMappings).UsingGet()).RespondWith(new DynamicResponseProvider(MappingsGet));
            Given(Request.Create().WithPath(AdminMappings).UsingPost().WithHeader(HttpKnownHeaderNames.ContentType, ContentTypeJson)).RespondWith(new DynamicResponseProvider(MappingsPost));
            Given(Request.Create().WithPath(AdminMappings).UsingDelete()).RespondWith(new DynamicResponseProvider(MappingsDelete));

            // __admin/mappings/reset
            Given(Request.Create().WithPath(AdminMappings + "/reset").UsingPost()).RespondWith(new DynamicResponseProvider(MappingsDelete));

            // __admin/mappings/{guid}
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingGet()).RespondWith(new DynamicResponseProvider(MappingGet));
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingPut().WithHeader(HttpKnownHeaderNames.ContentType, ContentTypeJson)).RespondWith(new DynamicResponseProvider(MappingPut));
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
        #endregion

        #region StaticMappings
        /// <summary>
        /// Reads the static mappings from a folder.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use \__admin\mappings\</param>
        [PublicAPI]
        public void ReadStaticMappings([CanBeNull] string folder = null)
        {
            if (folder == null)
            {
                folder = Path.Combine(Directory.GetCurrentDirectory(), AdminMappingsFolder);
            }

            if (!Directory.Exists(folder))
            {
                return;
            }

            foreach (string filename in Directory.EnumerateFiles(folder).OrderBy(f => f))
            {
                _logger.Info("Reading Static MappingFile : '{0}'", filename);
                ReadStaticMappingAndAddOrUpdate(filename);
            }
        }

        /// <summary>
        /// Watches the static mappings for changes.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use \__admin\mappings\</param>
        [PublicAPI]
        public void WatchStaticMappings([CanBeNull] string folder = null)
        {
            if (folder == null)
            {
                folder = Path.Combine(Directory.GetCurrentDirectory(), AdminMappingsFolder);
            }

            if (!Directory.Exists(folder))
            {
                return;
            }

            _logger.Info("Watching folder '{0}' for new, updated and deleted MappingFiles.", folder);

            var watcher = new EnhancedFileSystemWatcher(folder, "*.json", 1000);
            watcher.Created += (sender, args) =>
            {
                _logger.Info("New MappingFile created : '{0}'", args.FullPath);
                ReadStaticMappingAndAddOrUpdate(args.FullPath);
            };
            watcher.Changed += (sender, args) =>
            {
                _logger.Info("New MappingFile updated : '{0}'", args.FullPath);
                ReadStaticMappingAndAddOrUpdate(args.FullPath);
            };
            watcher.Deleted += (sender, args) =>
            {
                _logger.Info("New MappingFile deleted : '{0}'", args.FullPath);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(args.FullPath);

                if (Guid.TryParse(filenameWithoutExtension, out Guid guidFromFilename))
                {
                    DeleteMapping(guidFromFilename);
                }
                else
                {
                    DeleteMapping(args.FullPath);
                }
            };

            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Reads a static mapping file and adds or updates the mapping.
        /// </summary>
        /// <param name="path">The path.</param>
        [PublicAPI]
        public void ReadStaticMappingAndAddOrUpdate([NotNull] string path)
        {
            Check.NotNull(path, nameof(path));

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            MappingModel mappingModel = JsonConvert.DeserializeObject<MappingModel>(FileHelper.ReadAllText(path));
            if (Guid.TryParse(filenameWithoutExtension, out Guid guidFromFilename))
            {
                DeserializeAndAddOrUpdateMapping(mappingModel, guidFromFilename, path);
            }
            else
            {
                DeserializeAndAddOrUpdateMapping(mappingModel, null, path);
            }
        }
        #endregion

        #region Proxy and Record
        private HttpClient _httpClientForProxy;

        private void InitProxyAndRecord(IProxyAndRecordSettings settings)
        {
            _httpClientForProxy = HttpClientHelper.CreateHttpClient(settings.ClientX509Certificate2ThumbprintOrSubjectName);
            Given(Request.Create().WithPath("/*").UsingAnyMethod()).RespondWith(new ProxyAsyncResponseProvider(ProxyAndRecordAsync, settings));
        }

        private async Task<ResponseMessage> ProxyAndRecordAsync(RequestMessage requestMessage, IProxyAndRecordSettings settings)
        {
            var requestUri = new Uri(requestMessage.Url);
            var proxyUri = new Uri(settings.Url);
            var proxyUriWithRequestPathAndQuery = new Uri(proxyUri, requestUri.PathAndQuery);

            var responseMessage = await HttpClientHelper.SendAsync(_httpClientForProxy, requestMessage, proxyUriWithRequestPathAndQuery.AbsoluteUri);

            if (settings.SaveMapping)
            {
                var mapping = ToMapping(requestMessage, responseMessage, settings.BlackListedHeaders ?? new string[] { });
                _options.Mappings.Add(mapping.Guid, mapping);

                if (settings.SaveMappingToFile)
                {
                    SaveMappingToFile(mapping);
                }
            }

            return responseMessage;
        }

        private Mapping ToMapping(RequestMessage requestMessage, ResponseMessage responseMessage, string[] blacklistedHeaders)
        {
            var request = Request.Create();
            request.WithPath(requestMessage.Path);
            request.UsingMethod(requestMessage.Method);

            requestMessage.Query.Loop((key, value) => request.WithParam(key, value.ToArray()));
            requestMessage.Cookies.Loop((key, value) => request.WithCookie(key, value));

            var allBlackListedHeaders = new List<string>(blacklistedHeaders) { "Cookie" };
            requestMessage.Headers.Loop((key, value) =>
            {
                if (!allBlackListedHeaders.Any(b => string.Equals(key, b, StringComparison.OrdinalIgnoreCase)))
                {
                    request.WithHeader(key, value.ToArray());
                }
            });

            if (requestMessage.Body != null)
            {
                request.WithBody(new ExactMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.Body));
            }

            var response = Response.Create(responseMessage);

            return new Mapping(Guid.NewGuid(), string.Empty, null, request, response, 0, null, null, null);
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
            var settings = DeserializeObject<SettingsModel>(requestMessage);
            _options.MaxRequestLogCount = settings.MaxRequestLogCount;
            _options.RequestLogExpirationDuration = settings.RequestLogExpirationDuration;

            if (settings.AllowPartialMapping != null)
            {
                _options.AllowPartialMapping = settings.AllowPartialMapping.Value;
            }

            if (settings.GlobalProcessingDelay != null)
            {
                _options.RequestProcessingDelay = TimeSpan.FromMilliseconds(settings.GlobalProcessingDelay.Value);
            }

            return new ResponseMessage { Body = "Settings updated" };
        }
        #endregion Settings

        #region Mapping/{guid}
        private ResponseMessage MappingGet(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminMappings.Length + 1));
            var mapping = Mappings.FirstOrDefault(m => !m.IsAdminInterface && m.Guid == guid);

            if (mapping == null)
            {
                _logger.Warn("HttpStatusCode set to 404 : Mapping not found");
                return new ResponseMessage { StatusCode = 404, Body = "Mapping not found" };
            }

            var model = MappingConverter.ToMappingModel(mapping);

            return ToJson(model);
        }

        private ResponseMessage MappingPut(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.TrimStart(AdminMappings.ToCharArray()));

            var mappingModel = DeserializeObject<MappingModel>(requestMessage);
            DeserializeAndAddOrUpdateMapping(mappingModel, guid);

            return new ResponseMessage { Body = "Mapping added or updated" };
        }

        private ResponseMessage MappingDelete(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminMappings.Length + 1));

            if (DeleteMapping(guid))
            {
                return new ResponseMessage { Body = "Mapping removed" };
            }

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
            {
                Directory.CreateDirectory(folder);
            }

            var model = MappingConverter.ToMappingModel(mapping);
            string filename = !string.IsNullOrEmpty(mapping.Title) ? SanitizeFileName(mapping.Title) : mapping.Guid.ToString();

            string filePath = Path.Combine(folder, filename + ".json");
            _logger.Info("Saving Mapping to file {0}", filePath);

            File.WriteAllText(filePath, JsonConvert.SerializeObject(model, _settings));
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
                var mappingModel = DeserializeObject<MappingModel>(requestMessage);
                DeserializeAndAddOrUpdateMapping(mappingModel);
            }
            catch (ArgumentException a)
            {
                _logger.Error("HttpStatusCode set to 400 {0}", a);
                return new ResponseMessage { StatusCode = 400, Body = a.Message };
            }
            catch (Exception e)
            {
                _logger.Error("HttpStatusCode set to 500 {0}", e);
                return new ResponseMessage { StatusCode = 500, Body = e.ToString() };
            }

            return new ResponseMessage { StatusCode = 201, Body = "Mapping added" };
        }

        private void DeserializeAndAddOrUpdateMapping(MappingModel mappingModel, Guid? guid = null, string path = null)
        {
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

            if (path != null)
            {
                respondProvider = respondProvider.WithPath(path);
            }

            if (!string.IsNullOrEmpty(mappingModel.Title))
            {
                respondProvider = respondProvider.WithTitle(mappingModel.Title);
            }

            if (mappingModel.Priority != null)
            {
                respondProvider = respondProvider.AtPriority(mappingModel.Priority.Value);
            }

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
            {
                _logger.Warn("HttpStatusCode set to 404 : Request not found");
                return new ResponseMessage { StatusCode = 404, Body = "Request not found" };
            }

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
                    BodyAsJson = logEntry.RequestMessage.BodyAsJson,
                    BodyAsBytes = logEntry.RequestMessage.BodyAsBytes,
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
                    BodyAsJson = logEntry.ResponseMessage.BodyAsJson,
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
            var requestModel = DeserializeObject<RequestModel>(requestMessage);

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
                if (requestModel.ClientIP is string clientIP)
                {
                    requestBuilder = requestBuilder.WithClientIP(clientIP);
                }
                else
                {
                    var clientIPModel = JsonUtils.ParseJTokenToObject<ClientIPModel>(requestModel.ClientIP);
                    if (clientIPModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithPath(clientIPModel.Matchers.Select(MatcherModelMapper.Map).Cast<IStringMatcher>().ToArray());
                    }
                }
            }

            if (requestModel.Path != null)
            {
                if (requestModel.Path is string path)
                {
                    requestBuilder = requestBuilder.WithPath(path);
                }
                else
                {
                    var pathModel = JsonUtils.ParseJTokenToObject<PathModel>(requestModel.Path);
                    if (pathModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithPath(pathModel.Matchers.Select(MatcherModelMapper.Map).Cast<IStringMatcher>().ToArray());
                    }
                }
            }

            if (requestModel.Url != null)
            {
                if (requestModel.Url is string url)
                {
                    requestBuilder = requestBuilder.WithUrl(url);
                }
                else
                {
                    var urlModel = JsonUtils.ParseJTokenToObject<UrlModel>(requestModel.Url);
                    if (urlModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithUrl(urlModel.Matchers.Select(MatcherModelMapper.Map).Cast<IStringMatcher>().ToArray());
                    }
                }
            }

            if (requestModel.Methods != null)
            {
                requestBuilder = requestBuilder.UsingMethod(requestModel.Methods);
            }

            if (requestModel.Headers != null)
            {
                foreach (var headerModel in requestModel.Headers.Where(h => h.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithHeader(headerModel.Name, headerModel.Matchers.Select(MatcherModelMapper.Map).Cast<IStringMatcher>().ToArray());
                }
            }

            if (requestModel.Cookies != null)
            {
                foreach (var cookieModel in requestModel.Cookies.Where(c => c.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithCookie(cookieModel.Name, cookieModel.Matchers.Select(MatcherModelMapper.Map).Cast<IStringMatcher>().ToArray());
                }
            }

            if (requestModel.Params != null)
            {
                foreach (var paramModel in requestModel.Params)
                {
                    requestBuilder = paramModel.Values == null ? requestBuilder.WithParam(paramModel.Name) : requestBuilder.WithParam(paramModel.Name, paramModel.Values.ToArray());
                }
            }

            if (requestModel.Body?.Matcher != null)
            {
                var bodyMatcher = MatcherModelMapper.Map(requestModel.Body.Matcher);
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
                responseBuilder = responseBuilder.WithBodyAsJson(responseModel.BodyAsJson, ToEncoding(responseModel.BodyEncoding), responseModel.BodyAsJsonIndented == true);
            }
            else if (responseModel.BodyFromBase64 != null)
            {
                responseBuilder = responseBuilder.WithBodyFromBase64(responseModel.BodyFromBase64, ToEncoding(responseModel.BodyEncoding));
            }

            else if (responseModel.BodyAsFile != null)
            {
                responseBuilder = responseBuilder.WithBodyFromFile(responseModel.BodyAsFile);
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
                Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>("application/json") } }
            };
        }

        private Encoding ToEncoding(EncodingModel encodingModel)
        {
            return encodingModel != null ? Encoding.GetEncoding(encodingModel.CodePage) : null;
        }

        private T DeserializeObject<T>(RequestMessage requestMessage)
        {
            return requestMessage.Body != null ?
                JsonConvert.DeserializeObject<T>(requestMessage.Body) :
                ((JObject)requestMessage.BodyAsJson).ToObject<T>();
        }
    }
}
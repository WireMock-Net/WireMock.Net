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
using WireMock.Admin.Scenarios;
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
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly JsonSerializerSettings _settingsIncludeNullValues = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include
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
        /// Saves the static mappings.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
        [PublicAPI]
        public void SaveStaticMappings([CanBeNull] string folder = null)
        {
            foreach (var mapping in Mappings.Where(m => !m.IsAdminInterface))
            {
                SaveMappingToFile(mapping, folder);
            }
        }

        /// <summary>
        /// Reads the static mappings from a folder.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
        [PublicAPI]
        public void ReadStaticMappings([CanBeNull] string folder = null)
        {
            if (folder == null)
            {
                folder = _fileSystemHandler.GetMappingFolder();
            }

            if (!_fileSystemHandler.FolderExists(folder))
            {
                _logger.Info("The Static Mapping folder '{0}' does not exist, reading Static MappingFiles will be skipped.", folder);
                return;
            }

            foreach (string filename in _fileSystemHandler.EnumerateFiles(folder).OrderBy(f => f))
            {
                _logger.Info("Reading Static MappingFile : '{0}'", filename);

                try
                {
                    ReadStaticMappingAndAddOrUpdate(filename);
                }
                catch
                {
                    _logger.Error("Static MappingFile : '{0}' could not be read. This file will be skipped.", filename);
                }
            }
        }

        /// <summary>
        /// Watches the static mappings for changes.
        /// </summary>
        /// <param name="folder">The optional folder. If not defined, use {CurrentFolder}/__admin/mappings</param>
        [PublicAPI]
        public void WatchStaticMappings([CanBeNull] string folder = null)
        {
            if (folder == null)
            {
                folder = _fileSystemHandler.GetMappingFolder();
            }

            if (!_fileSystemHandler.FolderExists(folder))
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

            MappingModel mappingModel = JsonConvert.DeserializeObject<MappingModel>(_fileSystemHandler.ReadMappingFile(path));
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
                _options.Mappings.TryAdd(mapping.Guid, mapping);

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

            if (requestMessage.BodyAsJson != null)
            {
                request.WithBody(new JsonMatcher(MatchBehaviour.AcceptOnMatch, requestMessage.BodyAsJson));
            }
            else if (requestMessage.Body != null)
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

            return ResponseMessageBuilder.Create("Settings updated");
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
                return ResponseMessageBuilder.Create("Mapping not found", 404);
            }

            var model = MappingConverter.ToMappingModel(mapping);

            return ToJson(model);
        }

        private ResponseMessage MappingPut(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.TrimStart(AdminMappings.ToCharArray()));

            var mappingModel = DeserializeObject<MappingModel>(requestMessage);
            Guid? guidFromPut = DeserializeAndAddOrUpdateMapping(mappingModel, guid);

            return ResponseMessageBuilder.Create("Mapping added or updated", 200, guidFromPut);
        }

        private ResponseMessage MappingDelete(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminMappings.Length + 1));

            if (DeleteMapping(guid))
            {
                return ResponseMessageBuilder.Create("Mapping removed", 200, guid);
            }

            return ResponseMessageBuilder.Create("Mapping not found", 404);
        }
        #endregion Mapping/{guid}

        #region Mappings
        private ResponseMessage MappingsSave(RequestMessage requestMessage)
        {
            SaveStaticMappings();

            return ResponseMessageBuilder.Create("Mappings saved to disk");
        }

        private void SaveMappingToFile(Mapping mapping, string folder = null)
        {
            if (folder == null)
            {
                folder = _fileSystemHandler.GetMappingFolder();
            }

            if (!_fileSystemHandler.FolderExists(folder))
            {
                _fileSystemHandler.CreateFolder(folder);
            }

            var model = MappingConverter.ToMappingModel(mapping);
            string filename = (!string.IsNullOrEmpty(mapping.Title) ? SanitizeFileName(mapping.Title) : mapping.Guid.ToString()) + ".json";

            string path = Path.Combine(folder, filename);

            _logger.Info("Saving Mapping file {0}", filename);

            _fileSystemHandler.WriteMappingFile(path, JsonConvert.SerializeObject(model, _settings));
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
            Guid? guid;
            try
            {
                var mappingModel = DeserializeObject<MappingModel>(requestMessage);
                guid = DeserializeAndAddOrUpdateMapping(mappingModel);
            }
            catch (ArgumentException a)
            {
                _logger.Error("HttpStatusCode set to 400 {0}", a);
                return ResponseMessageBuilder.Create(a.Message, 400);
            }
            catch (Exception e)
            {
                _logger.Error("HttpStatusCode set to 500 {0}", e);
                return ResponseMessageBuilder.Create(e.ToString(), 500);
            }

            return ResponseMessageBuilder.Create("Mapping added", 201, guid);
        }

        private Guid? DeserializeAndAddOrUpdateMapping(MappingModel mappingModel, Guid? guid = null, string path = null)
        {
            Check.NotNull(mappingModel, nameof(mappingModel));
            Check.NotNull(mappingModel.Request, nameof(mappingModel.Request));
            Check.NotNull(mappingModel.Response, nameof(mappingModel.Response));

            var requestBuilder = InitRequestBuilder(mappingModel.Request, true);
            if (requestBuilder == null)
            {
                return null;
            }

            var responseBuilder = InitResponseBuilder(mappingModel.Response);

            var respondProvider = Given(requestBuilder);

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

            return respondProvider.Guid;
        }

        private ResponseMessage MappingsDelete(RequestMessage requestMessage)
        {
            ResetMappings();

            ResetScenarios();

            return ResponseMessageBuilder.Create("Mappings deleted");
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
                return ResponseMessageBuilder.Create("Request not found", 404);
            }

            var model = LogEntryMapper.Map(entry);

            return ToJson(model);
        }

        private ResponseMessage RequestDelete(RequestMessage requestMessage)
        {
            Guid guid = Guid.Parse(requestMessage.Path.Substring(AdminRequests.Length + 1));

            if (DeleteLogEntry(guid))
            {
                return ResponseMessageBuilder.Create("Request removed");
            }

            return ResponseMessageBuilder.Create("Request not found", 404);
        }
        #endregion Request/{guid}

        #region Requests
        private ResponseMessage RequestsGet(RequestMessage requestMessage)
        {
            var result = LogEntries
                .Where(r => !r.RequestMessage.Path.StartsWith("/__admin/"))
                .Select(LogEntryMapper.Map);

            return ToJson(result);
        }

        private ResponseMessage RequestsDelete(RequestMessage requestMessage)
        {
            ResetLogEntries();

            return ResponseMessageBuilder.Create("Requests deleted");
        }
        #endregion Requests

        #region Requests/find
        private ResponseMessage RequestsFind(RequestMessage requestMessage)
        {
            var requestModel = DeserializeObject<RequestModel>(requestMessage);

            var request = (Request)InitRequestBuilder(requestModel, false);

            var dict = new Dictionary<LogEntry, RequestMatchResult>();
            foreach (var logEntry in LogEntries.Where(le => !le.RequestMessage.Path.StartsWith("/__admin/")))
            {
                var requestMatchResult = new RequestMatchResult();
                if (request.GetMatchingScore(logEntry.RequestMessage, requestMatchResult) > MatchScores.AlmostPerfect)
                {
                    dict.Add(logEntry, requestMatchResult);
                }
            }

            var result = dict.OrderBy(x => x.Value.AverageTotalScore).Select(x => x.Key).Select(LogEntryMapper.Map);

            return ToJson(result);
        }
        #endregion Requests/find

        #region Scenarios
        private ResponseMessage ScenariosGet(RequestMessage requestMessage)
        {
            var scenariosStates = Scenarios.Values.Select(s => new ScenarioStateModel
            {
                Name = s.Name,
                NextState = s.NextState,
                Started = s.Started,
                Finished = s.Finished
            });

            return ToJson(scenariosStates, true);
        }

        private ResponseMessage ScenariosReset(RequestMessage requestMessage)
        {
            ResetScenarios();

            return ResponseMessageBuilder.Create("Scenarios reset");
        }
        #endregion

        private IRequestBuilder InitRequestBuilder(RequestModel requestModel, bool pathOrUrlRequired)
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
                        requestBuilder = requestBuilder.WithPath(clientIPModel.Matchers.Select(MatcherMapper.Map).Cast<IStringMatcher>().ToArray());
                    }
                }
            }

            bool pathOrUrlmatchersValid = false;
            if (requestModel.Path != null)
            {
                if (requestModel.Path is string path)
                {
                    requestBuilder = requestBuilder.WithPath(path);
                    pathOrUrlmatchersValid = true;
                }
                else
                {
                    var pathModel = JsonUtils.ParseJTokenToObject<PathModel>(requestModel.Path);
                    if (pathModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithPath(pathModel.Matchers.Select(MatcherMapper.Map).Cast<IStringMatcher>().ToArray());
                        pathOrUrlmatchersValid = true;
                    }
                }
            }
            else if (requestModel.Url != null)
            {
                if (requestModel.Url is string url)
                {
                    requestBuilder = requestBuilder.WithUrl(url);
                    pathOrUrlmatchersValid = true;
                }
                else
                {
                    var urlModel = JsonUtils.ParseJTokenToObject<UrlModel>(requestModel.Url);
                    if (urlModel?.Matchers != null)
                    {
                        requestBuilder = requestBuilder.WithUrl(urlModel.Matchers.Select(MatcherMapper.Map).Cast<IStringMatcher>().ToArray());
                        pathOrUrlmatchersValid = true;
                    }
                }
            }

            if (pathOrUrlRequired && !pathOrUrlmatchersValid)
            {
                _logger.Error("Path or Url matcher is missing for this mapping, this mapping will not be added.");
                return null;
            }

            if (requestModel.Methods != null)
            {
                requestBuilder = requestBuilder.UsingMethod(requestModel.Methods);
            }

            if (requestModel.Headers != null)
            {
                foreach (var headerModel in requestModel.Headers.Where(h => h.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithHeader(headerModel.Name, headerModel.Matchers.Select(MatcherMapper.Map).Cast<IStringMatcher>().ToArray());
                }
            }

            if (requestModel.Cookies != null)
            {
                foreach (var cookieModel in requestModel.Cookies.Where(c => c.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithCookie(cookieModel.Name, cookieModel.Matchers.Select(MatcherMapper.Map).Cast<IStringMatcher>().ToArray());
                }
            }

            if (requestModel.Params != null)
            {
                foreach (var paramModel in requestModel.Params.Where(c => c.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithParam(paramModel.Name, paramModel.Matchers.Select(MatcherMapper.Map).Cast<IStringMatcher>().ToArray());
                }
            }

            if (requestModel.Body?.Matcher != null)
            {
                var bodyMatcher = MatcherMapper.Map(requestModel.Body.Matcher);
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

        private ResponseMessage ToJson<T>(T result, bool keepNullValues = false)
        {
            return new ResponseMessage
            {
                Body = JsonConvert.SerializeObject(result, keepNullValues ? _settingsIncludeNullValues : _settings),
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
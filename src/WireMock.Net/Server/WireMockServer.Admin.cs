using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using WireMock.Models;
using WireMock.Proxy;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Server
{
    /// <summary>
    /// The fluent mock server.
    /// </summary>
    public partial class WireMockServer
    {
        private const int EnhancedFileSystemWatcherTimeoutMs = 1000;
        private const int AdminPriority = int.MinValue;
        private const int ProxyPriority = 1000;
        private const string ContentTypeJson = "application/json";
        private const string AdminFiles = "/__admin/files";
        private const string AdminMappings = "/__admin/mappings";
        private const string AdminRequests = "/__admin/requests";
        private const string AdminSettings = "/__admin/settings";
        private const string AdminScenarios = "/__admin/scenarios";
        private const string QueryParamReloadStaticMappings = "reloadStaticMappings";

        private readonly RegexMatcher _adminRequestContentTypeJson = new ContentTypeMatcher(ContentTypeJson, true);
        private readonly RegexMatcher _adminMappingsGuidPathMatcher = new RegexMatcher(@"^\/__admin\/mappings\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");
        private readonly RegexMatcher _adminRequestsGuidPathMatcher = new RegexMatcher(@"^\/__admin\/requests\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");

        #region InitAdmin
        private void InitAdmin()
        {
            // __admin/settings
            Given(Request.Create().WithPath(AdminSettings).UsingGet()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(SettingsGet));
            Given(Request.Create().WithPath(AdminSettings).UsingMethod("PUT", "POST").WithHeader(HttpKnownHeaderNames.ContentType, _adminRequestContentTypeJson)).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(SettingsUpdate));

            // __admin/mappings
            Given(Request.Create().WithPath(AdminMappings).UsingGet()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingsGet));
            Given(Request.Create().WithPath(AdminMappings).UsingPost().WithHeader(HttpKnownHeaderNames.ContentType, _adminRequestContentTypeJson)).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingsPost));
            Given(Request.Create().WithPath(AdminMappings).UsingDelete()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingsDelete));

            // __admin/mappings/reset
            Given(Request.Create().WithPath(AdminMappings + "/reset").UsingPost()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingsReset));

            // __admin/mappings/{guid}
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingGet()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingGet));
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingPut().WithHeader(HttpKnownHeaderNames.ContentType, _adminRequestContentTypeJson)).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingPut));
            Given(Request.Create().WithPath(_adminMappingsGuidPathMatcher).UsingDelete()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingDelete));

            // __admin/mappings/save
            Given(Request.Create().WithPath(AdminMappings + "/save").UsingPost()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(MappingsSave));

            // __admin/requests
            Given(Request.Create().WithPath(AdminRequests).UsingGet()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(RequestsGet));
            Given(Request.Create().WithPath(AdminRequests).UsingDelete()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(RequestsDelete));

            // __admin/requests/reset
            Given(Request.Create().WithPath(AdminRequests + "/reset").UsingPost()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(RequestsDelete));

            // __admin/request/{guid}
            Given(Request.Create().WithPath(_adminRequestsGuidPathMatcher).UsingGet()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(RequestGet));
            Given(Request.Create().WithPath(_adminRequestsGuidPathMatcher).UsingDelete()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(RequestDelete));

            // __admin/requests/find
            Given(Request.Create().WithPath(AdminRequests + "/find").UsingPost()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(RequestsFind));

            // __admin/scenarios
            Given(Request.Create().WithPath(AdminScenarios).UsingGet()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosGet));
            Given(Request.Create().WithPath(AdminScenarios).UsingDelete()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosReset));

            // __admin/scenarios/reset
            Given(Request.Create().WithPath(AdminScenarios + "/reset").UsingPost()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosReset));

            // __admin/files/{filename}
            Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingPost()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(FilePost));
            Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingPut()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(FilePut));
            Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingGet()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(FileGet));
            Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingHead()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(FileHead));
            Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingDelete()).AtPriority(AdminPriority).RespondWith(new DynamicResponseProvider(FileDelete));
        }
        #endregion

        #region StaticMappings
        /// <inheritdoc cref="IWireMockServer.SaveStaticMappings" />
        [PublicAPI]
        public void SaveStaticMappings([CanBeNull] string folder = null)
        {
            foreach (var mapping in Mappings.Where(m => !m.IsAdminInterface))
            {
                _mappingToFileSaver.SaveMappingToFile(mapping, folder);
            }
        }

        /// <inheritdoc cref="IWireMockServer.ReadStaticMappings" />
        [PublicAPI]
        public void ReadStaticMappings([CanBeNull] string folder = null)
        {
            if (folder == null)
            {
                folder = _settings.FileSystemHandler.GetMappingFolder();
            }

            if (!_settings.FileSystemHandler.FolderExists(folder))
            {
                _settings.Logger.Info("The Static Mapping folder '{0}' does not exist, reading Static MappingFiles will be skipped.", folder);
                return;
            }

            foreach (string filename in _settings.FileSystemHandler.EnumerateFiles(folder, _settings.WatchStaticMappingsInSubdirectories == true).OrderBy(f => f))
            {
                _settings.Logger.Info("Reading Static MappingFile : '{0}'", filename);

                try
                {
                    ReadStaticMappingAndAddOrUpdate(filename);
                }
                catch
                {
                    _settings.Logger.Error("Static MappingFile : '{0}' could not be read. This file will be skipped.", filename);
                }
            }
        }

        /// <inheritdoc cref="IWireMockServer.WatchStaticMappings" />
        [PublicAPI]
        public void WatchStaticMappings([CanBeNull] string folder = null)
        {
            if (folder == null)
            {
                folder = _settings.FileSystemHandler.GetMappingFolder();
            }

            if (!_settings.FileSystemHandler.FolderExists(folder))
            {
                return;
            }

            bool includeSubdirectories = _settings.WatchStaticMappingsInSubdirectories == true;
            string includeSubdirectoriesText = includeSubdirectories ? " and Subdirectories" : string.Empty;

            _settings.Logger.Info($"Watching folder '{folder}'{includeSubdirectoriesText} for new, updated and deleted MappingFiles.");

            var watcher = new EnhancedFileSystemWatcher(folder, "*.json", EnhancedFileSystemWatcherTimeoutMs)
            {
                IncludeSubdirectories = includeSubdirectories
            };

            watcher.Created += (sender, args) =>
            {
                _settings.Logger.Info("MappingFile created : '{0}', reading file.", args.FullPath);
                if (!ReadStaticMappingAndAddOrUpdate(args.FullPath))
                {
                    _settings.Logger.Error("Unable to read MappingFile '{0}'.", args.FullPath);
                }
            };
            watcher.Changed += (sender, args) =>
            {
                _settings.Logger.Info("MappingFile updated : '{0}', reading file.", args.FullPath);
                if (!ReadStaticMappingAndAddOrUpdate(args.FullPath))
                {
                    _settings.Logger.Error("Unable to read MappingFile '{0}'.", args.FullPath);
                }
            };
            watcher.Deleted += (sender, args) =>
            {
                _settings.Logger.Info("MappingFile deleted : '{0}'", args.FullPath);
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

        /// <inheritdoc cref="IWireMockServer.WatchStaticMappings" />
        [PublicAPI]
        public bool ReadStaticMappingAndAddOrUpdate([NotNull] string path)
        {
            Check.NotNull(path, nameof(path));

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            if (FileHelper.TryReadMappingFileWithRetryAndDelay(_settings.FileSystemHandler, path, out string value))
            {
                var mappingModels = DeserializeJsonToArray<MappingModel>(value);
                foreach (var mappingModel in mappingModels)
                {
                    if (mappingModels.Length == 1 && Guid.TryParse(filenameWithoutExtension, out Guid guidFromFilename))
                    {
                        ConvertMappingAndRegisterAsRespondProvider(mappingModel, guidFromFilename, path);
                    }
                    else
                    {
                        ConvertMappingAndRegisterAsRespondProvider(mappingModel, null, path);
                    }
                }

                return true;
            }

            return false;
        }
        #endregion

        #region Proxy and Record
        private HttpClient _httpClientForProxy;

        private void InitProxyAndRecord(IWireMockServerSettings settings)
        {
            _httpClientForProxy = HttpClientBuilder.Build(settings.ProxyAndRecordSettings);

            var respondProvider = Given(Request.Create().WithPath("/*").UsingAnyMethod());
            if (settings.StartAdminInterface == true)
            {
                respondProvider.AtPriority(ProxyPriority);
            }

            respondProvider.RespondWith(new ProxyAsyncResponseProvider(ProxyAndRecordAsync, settings));
        }

        private async Task<ResponseMessage> ProxyAndRecordAsync(RequestMessage requestMessage, IWireMockServerSettings settings)
        {
            var requestUri = new Uri(requestMessage.Url);
            var proxyUri = new Uri(settings.ProxyAndRecordSettings.Url);
            var proxyUriWithRequestPathAndQuery = new Uri(proxyUri, requestUri.PathAndQuery);

            var proxyHelper = new ProxyHelper(settings);

            var (responseMessage, mapping) = await proxyHelper.SendAsync(
                _settings.ProxyAndRecordSettings,
                _httpClientForProxy,
                requestMessage,
                proxyUriWithRequestPathAndQuery.AbsoluteUri
            );

            if (mapping != null)
            {
                if (settings.ProxyAndRecordSettings.SaveMapping)
                {
                    _options.Mappings.TryAdd(mapping.Guid, mapping);
                }

                if (settings.ProxyAndRecordSettings.SaveMappingToFile)
                {
                    _mappingToFileSaver.SaveMappingToFile(mapping);
                }
            }

            return responseMessage;
        }
        #endregion

        #region Settings
        private ResponseMessage SettingsGet(RequestMessage requestMessage)
        {
            var model = new SettingsModel
            {
                AllowPartialMapping = _settings.AllowPartialMapping,
                MaxRequestLogCount = _settings.MaxRequestLogCount,
                RequestLogExpirationDuration = _settings.RequestLogExpirationDuration,
                GlobalProcessingDelay = (int?)_options.RequestProcessingDelay?.TotalMilliseconds,
                AllowBodyForAllHttpMethods = _settings.AllowBodyForAllHttpMethods,
                HandleRequestsSynchronously = _settings.HandleRequestsSynchronously,
                ThrowExceptionWhenMatcherFails = _settings.ThrowExceptionWhenMatcherFails
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

            if (settings.AllowBodyForAllHttpMethods != null)
            {
                _options.AllowBodyForAllHttpMethods = settings.AllowBodyForAllHttpMethods.Value;
            }

            if (settings.HandleRequestsSynchronously != null)
            {
                _options.HandleRequestsSynchronously = settings.HandleRequestsSynchronously.Value;
            }

            if (settings.ThrowExceptionWhenMatcherFails != null)
            {
                _settings.ThrowExceptionWhenMatcherFails = settings.ThrowExceptionWhenMatcherFails.Value;
            }

            return ResponseMessageBuilder.Create("Settings updated");
        }
        #endregion Settings

        #region Mapping/{guid}
        private ResponseMessage MappingGet(RequestMessage requestMessage)
        {
            Guid guid = ParseGuidFromRequestMessage(requestMessage);
            var mapping = Mappings.FirstOrDefault(m => !m.IsAdminInterface && m.Guid == guid);

            if (mapping == null)
            {
                _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
                return ResponseMessageBuilder.Create("Mapping not found", 404);
            }

            var model = _mappingConverter.ToMappingModel(mapping);

            return ToJson(model);
        }

        private ResponseMessage MappingPut(RequestMessage requestMessage)
        {
            Guid guid = ParseGuidFromRequestMessage(requestMessage);

            var mappingModel = DeserializeObject<MappingModel>(requestMessage);
            Guid? guidFromPut = ConvertMappingAndRegisterAsRespondProvider(mappingModel, guid);

            return ResponseMessageBuilder.Create("Mapping added or updated", 200, guidFromPut);
        }

        private ResponseMessage MappingDelete(RequestMessage requestMessage)
        {
            Guid guid = ParseGuidFromRequestMessage(requestMessage);

            if (DeleteMapping(guid))
            {
                return ResponseMessageBuilder.Create("Mapping removed", 200, guid);
            }

            return ResponseMessageBuilder.Create("Mapping not found", 404);
        }

        private Guid ParseGuidFromRequestMessage(RequestMessage requestMessage)
        {
            return Guid.Parse(requestMessage.Path.Substring(AdminMappings.Length + 1));
        }
        #endregion Mapping/{guid}

        #region Mappings
        private ResponseMessage MappingsSave(RequestMessage requestMessage)
        {
            SaveStaticMappings();

            return ResponseMessageBuilder.Create("Mappings saved to disk");
        }

        private IEnumerable<MappingModel> ToMappingModels()
        {
            return Mappings.Where(m => !m.IsAdminInterface).Select(_mappingConverter.ToMappingModel);
        }

        private ResponseMessage MappingsGet(RequestMessage requestMessage)
        {
            return ToJson(ToMappingModels());
        }

        private ResponseMessage MappingsPost(RequestMessage requestMessage)
        {
            try
            {
                var mappingModels = DeserializeRequestMessageToArray<MappingModel>(requestMessage);
                if (mappingModels.Length == 1)
                {
                    Guid? guid = ConvertMappingAndRegisterAsRespondProvider(mappingModels[0]);
                    return ResponseMessageBuilder.Create("Mapping added", 201, guid);
                }

                foreach (var mappingModel in mappingModels)
                {
                    ConvertMappingAndRegisterAsRespondProvider(mappingModel);
                }

                return ResponseMessageBuilder.Create("Mappings added", 201);
            }
            catch (ArgumentException a)
            {
                _settings.Logger.Error("HttpStatusCode set to 400 {0}", a);
                return ResponseMessageBuilder.Create(a.Message, 400);
            }
            catch (Exception e)
            {
                _settings.Logger.Error("HttpStatusCode set to 500 {0}", e);
                return ResponseMessageBuilder.Create(e.ToString(), 500);
            }
        }

        private Guid? ConvertMappingAndRegisterAsRespondProvider(MappingModel mappingModel, Guid? guid = null, string path = null)
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

            var respondProvider = Given(requestBuilder, mappingModel.SaveToFile == true);

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

            if (mappingModel.Webhook?.Request != null)
            {
                respondProvider = respondProvider.WithWebhook(Map(mappingModel.Webhook));
            }

            respondProvider.RespondWith(responseBuilder);

            return respondProvider.Guid;
        }

        private ResponseMessage MappingsDelete(RequestMessage requestMessage)
        {
            if (!string.IsNullOrEmpty(requestMessage.Body))
            {
                var deletedGuids = MappingsDeleteMappingFromBody(requestMessage);
                if (deletedGuids != null)
                {
                    return ResponseMessageBuilder.Create($"Mappings deleted. Affected GUIDs: [{string.Join(", ", deletedGuids.ToArray())}]");
                }
                else
                {
                    // return bad request
                    return ResponseMessageBuilder.Create("Poorly formed mapping JSON.", 400);
                }
            }
            else
            {
                ResetMappings();

                ResetScenarios();

                return ResponseMessageBuilder.Create("Mappings deleted");
            }
        }

        private IEnumerable<Guid> MappingsDeleteMappingFromBody(RequestMessage requestMessage)
        {
            var deletedGuids = new List<Guid>();

            try
            {
                var mappingModels = DeserializeRequestMessageToArray<MappingModel>(requestMessage);
                foreach (var mappingModel in mappingModels)
                {
                    if (mappingModel.Guid.HasValue)
                    {
                        if (DeleteMapping(mappingModel.Guid.Value))
                        {
                            deletedGuids.Add(mappingModel.Guid.Value);
                        }
                        else
                        {
                            _settings.Logger.Debug($"Did not find/delete mapping with GUID: {mappingModel.Guid.Value}.");
                        }
                    }
                }
            }
            catch (ArgumentException a)
            {
                _settings.Logger.Error("ArgumentException: {0}", a);
                return null;
            }
            catch (Exception e)
            {
                _settings.Logger.Error("Exception: {0}", e);
                return null;
            }

            return deletedGuids;
        }

        private ResponseMessage MappingsReset(RequestMessage requestMessage)
        {
            ResetMappings();

            ResetScenarios();

            string message = "Mappings reset";
            if (requestMessage.Query.ContainsKey(QueryParamReloadStaticMappings) &&
                bool.TryParse(requestMessage.Query[QueryParamReloadStaticMappings].ToString(), out bool reloadStaticMappings)
                && reloadStaticMappings)
            {
                ReadStaticMappings();
                message = $"{message} and static mappings reloaded";
            }

            return ResponseMessageBuilder.Create(message);
        }
        #endregion Mappings

        #region Request/{guid}
        private ResponseMessage RequestGet(RequestMessage requestMessage)
        {
            Guid guid = ParseGuidFromRequestMessage(requestMessage);
            var entry = LogEntries.FirstOrDefault(r => !r.RequestMessage.Path.StartsWith("/__admin/") && r.Guid == guid);

            if (entry == null)
            {
                _settings.Logger.Warn("HttpStatusCode set to 404 : Request not found");
                return ResponseMessageBuilder.Create("Request not found", 404);
            }

            var model = LogEntryMapper.Map(entry);

            return ToJson(model);
        }

        private ResponseMessage RequestDelete(RequestMessage requestMessage)
        {
            Guid guid = ParseGuidFromRequestMessage(requestMessage);

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

            var dict = new Dictionary<ILogEntry, RequestMatchResult>();
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
                Finished = s.Finished,
                Counter = s.Counter
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
                        requestBuilder = requestBuilder.WithPath(clientIPModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
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
                        requestBuilder = requestBuilder.WithPath(pathModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
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
                        requestBuilder = requestBuilder.WithUrl(urlModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
                        pathOrUrlmatchersValid = true;
                    }
                }
            }

            if (pathOrUrlRequired && !pathOrUrlmatchersValid)
            {
                _settings.Logger.Error("Path or Url matcher is missing for this mapping, this mapping will not be added.");
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
                    requestBuilder = requestBuilder.WithHeader(
                        headerModel.Name,
                        headerModel.IgnoreCase == true,
                        headerModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                        headerModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray()
                    );
                }
            }

            if (requestModel.Cookies != null)
            {
                foreach (var cookieModel in requestModel.Cookies.Where(c => c.Matchers != null))
                {
                    requestBuilder = requestBuilder.WithCookie(
                        cookieModel.Name,
                        cookieModel.IgnoreCase == true,
                        cookieModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                        cookieModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
                }
            }

            if (requestModel.Params != null)
            {
                foreach (var paramModel in requestModel.Params.Where(p => p != null && p.Matchers != null))
                {
                    bool ignoreCase = paramModel.IgnoreCase == true;
                    requestBuilder = requestBuilder.WithParam(paramModel.Name, ignoreCase, paramModel.Matchers.Select(_matcherMapper.Map).OfType<IStringMatcher>().ToArray());
                }
            }

            if (requestModel.Body?.Matcher != null)
            {
                requestBuilder = requestBuilder.WithBody(_matcherMapper.Map(requestModel.Body.Matcher));
            }
            else if (requestModel.Body?.Matchers != null)
            {
                requestBuilder = requestBuilder.WithBody(_matcherMapper.Map(requestModel.Body.Matchers));
            }

            return requestBuilder;
        }

        private IWebhook Map(WebhookModel model)
        {
            var webhook =  new Webhook
            {
                Request = new WebhookRequest
                {
                    Url = model.Request.Url,
                    Method = model.Request.Method,
                    Headers = model.Request.Headers != null ? model.Request.Headers.ToDictionary(x => x.Key, x => new WireMockList<string>(x.Value)) : null,
                    UseTransformer = model.Request.UseTransformer                    
                }
            };

            if (model.Request.UseTransformer == true)
            {
                if (!Enum.TryParse<TransformerType>(model.Request.TransformerType, out var transformerType))
                {
                    transformerType = TransformerType.Handlebars;
                }
                webhook.Request.TransformerType = transformerType;
            }

            return webhook;
        }

        private IResponseBuilder InitResponseBuilder(ResponseModel responseModel)
        {
            IResponseBuilder responseBuilder = Response.Create();

            if (responseModel.Delay > 0)
            {
                responseBuilder = responseBuilder.WithDelay(responseModel.Delay.Value);
            }

            if (responseModel.UseTransformer == true)
            {
                if (!Enum.TryParse<TransformerType>(responseModel.TransformerType, out var transformerType))
                {
                    transformerType = TransformerType.Handlebars;
                }
                responseBuilder = responseBuilder.WithTransformer(transformerType, responseModel.UseTransformerForBodyAsFile == true);
            }

            if (!string.IsNullOrEmpty(responseModel.ProxyUrl))
            {
                var proxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = responseModel.ProxyUrl,
                    ClientX509Certificate2ThumbprintOrSubjectName = responseModel.X509Certificate2ThumbprintOrSubjectName,
                    WebProxySettings = responseModel.WebProxy != null ? new WebProxySettings
                    {
                        Address = responseModel.WebProxy.Address,
                        UserName = responseModel.WebProxy.UserName,
                        Password = responseModel.WebProxy.Password
                    } : null
                };

                return responseBuilder.WithProxy(proxyAndRecordSettings);
            }

            if (responseModel.StatusCode is string statusCodeAsString)
            {
                responseBuilder = responseBuilder.WithStatusCode(statusCodeAsString);
            }
            else if (responseModel.StatusCode != null)
            {
                // Convert to Int32 because Newtonsoft deserializes an 'object' with a number value to a long.
                responseBuilder = responseBuilder.WithStatusCode(Convert.ToInt32(responseModel.StatusCode));
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
            else if (responseModel.BodyAsFile != null)
            {
                responseBuilder = responseBuilder.WithBodyFromFile(responseModel.BodyAsFile);
            }

            if (responseModel.Fault != null && Enum.TryParse(responseModel.Fault.Type, out FaultType faultType))
            {
                responseBuilder.WithFault(faultType, responseModel.Fault.Percentage);
            }

            return responseBuilder;
        }

        private ResponseMessage ToJson<T>(T result, bool keepNullValues = false)
        {
            return new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = JsonConvert.SerializeObject(result, keepNullValues ? JsonSerializationConstants.JsonSerializerSettingsIncludeNullValues : JsonSerializationConstants.JsonSerializerSettingsDefault)
                },
                StatusCode = (int)HttpStatusCode.OK,
                Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(ContentTypeJson) } }
            };
        }

        private Encoding ToEncoding(EncodingModel encodingModel)
        {
            return encodingModel != null ? Encoding.GetEncoding(encodingModel.CodePage) : null;
        }

        private T DeserializeObject<T>(RequestMessage requestMessage)
        {
            if (requestMessage?.BodyData?.DetectedBodyType == BodyType.String)
            {
                return JsonUtils.DeserializeObject<T>(requestMessage.BodyData.BodyAsString);
            }

            if (requestMessage?.BodyData?.DetectedBodyType == BodyType.Json)
            {
                return ((JObject)requestMessage.BodyData.BodyAsJson).ToObject<T>();
            }

            return default(T);
        }

        private T[] DeserializeRequestMessageToArray<T>(RequestMessage requestMessage)
        {
            if (requestMessage?.BodyData?.DetectedBodyType == BodyType.Json)
            {
                var bodyAsJson = requestMessage.BodyData.BodyAsJson;

                return DeserializeObjectToArray<T>(bodyAsJson);
            }

            return default(T[]);
        }

        private T[] DeserializeObjectToArray<T>(object value)
        {
            if (value is JArray jArray)
            {
                return jArray.ToObject<T[]>();
            }

            var singleResult = ((JObject)value).ToObject<T>();
            return new[] { singleResult };
        }

        private T[] DeserializeJsonToArray<T>(string value)
        {
            return DeserializeObjectToArray<T>(JsonUtils.DeserializeObject(value));
        }
    }
}
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
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Admin.Scenarios;
using WireMock.Admin.Settings;
using WireMock.Constants;
using WireMock.Http;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Proxy;
using WireMock.RequestBuilders;
using WireMock.ResponseProviders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Server;

/// <summary>
/// The fluent mock server.
/// </summary>
public partial class WireMockServer
{
    private const int EnhancedFileSystemWatcherTimeoutMs = 1000;
    private const string AdminFiles = "/__admin/files";
    private const string AdminMappings = "/__admin/mappings";
    private const string AdminMappingsWireMockOrg = "/__admin/mappings/wiremock.org";
    private const string AdminRequests = "/__admin/requests";
    private const string AdminSettings = "/__admin/settings";
    private const string AdminScenarios = "/__admin/scenarios";
    private const string QueryParamReloadStaticMappings = "reloadStaticMappings";

    private static readonly Guid ProxyMappingGuid = new("e59914fd-782e-428e-91c1-4810ffb86567");
    private static readonly RegexMatcher AdminRequestContentTypeJson = new ContentTypeMatcher(WireMockConstants.ContentTypeJson, true);
    private static readonly RegexMatcher AdminMappingsGuidPathMatcher = new(@"^\/__admin\/mappings\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");
    private static readonly RegexMatcher AdminRequestsGuidPathMatcher = new(@"^\/__admin\/requests\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");

    private EnhancedFileSystemWatcher? _enhancedFileSystemWatcher;

    #region InitAdmin
    private void InitAdmin()
    {
        // __admin/settings
        Given(Request.Create().WithPath(AdminSettings).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(SettingsGet));
        Given(Request.Create().WithPath(AdminSettings).UsingMethod("PUT", "POST").WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(SettingsUpdate));

        // __admin/mappings
        Given(Request.Create().WithPath(AdminMappings).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsGet));
        Given(Request.Create().WithPath(AdminMappings).UsingPost().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsPost));
        Given(Request.Create().WithPath(AdminMappingsWireMockOrg).UsingPost().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsPostWireMockOrg));
        Given(Request.Create().WithPath(AdminMappings).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsDelete));

        // __admin/mappings/reset
        Given(Request.Create().WithPath(AdminMappings + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsReset));

        // __admin/mappings/{guid}
        Given(Request.Create().WithPath(AdminMappingsGuidPathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingGet));
        Given(Request.Create().WithPath(AdminMappingsGuidPathMatcher).UsingPut().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingPut));
        Given(Request.Create().WithPath(AdminMappingsGuidPathMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingDelete));

        // __admin/mappings/save
        Given(Request.Create().WithPath($"{AdminMappings}/save").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsSave));

        // __admin/mappings/swagger
        Given(Request.Create().WithPath($"{AdminMappings}/swagger").UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(SwaggerGet));

        // __admin/requests
        Given(Request.Create().WithPath(AdminRequests).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsGet));
        Given(Request.Create().WithPath(AdminRequests).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsDelete));

        // __admin/requests/reset
        Given(Request.Create().WithPath(AdminRequests + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsDelete));

        // __admin/request/{guid}
        Given(Request.Create().WithPath(AdminRequestsGuidPathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestGet));
        Given(Request.Create().WithPath(AdminRequestsGuidPathMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestDelete));

        // __admin/requests/find
        Given(Request.Create().WithPath(AdminRequests + "/find").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsFind));

        // __admin/scenarios
        Given(Request.Create().WithPath(AdminScenarios).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosGet));
        Given(Request.Create().WithPath(AdminScenarios).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosReset));

        // __admin/scenarios/reset
        Given(Request.Create().WithPath(AdminScenarios + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosReset));

        // __admin/files/{filename}
        Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FilePost));
        Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingPut()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FilePut));
        Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FileGet));
        Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingHead()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FileHead));
        Given(Request.Create().WithPath(_adminFilesFilenamePathMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FileDelete));
    }
    #endregion

    #region StaticMappings
    /// <inheritdoc cref="IWireMockServer.SaveStaticMappings" />
    [PublicAPI]
    public void SaveStaticMappings(string? folder = null)
    {
        foreach (var mapping in Mappings.Where(m => !m.IsAdminInterface))
        {
            _mappingToFileSaver.SaveMappingToFile(mapping, folder);
        }
    }

    /// <inheritdoc cref="IWireMockServer.ReadStaticMappings" />
    [PublicAPI]
    public void ReadStaticMappings(string? folder = null)
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
    public void WatchStaticMappings(string? folder = null)
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

        DisposeEnhancedFileSystemWatcher();
        _enhancedFileSystemWatcher = new EnhancedFileSystemWatcher(folder, "*.json", EnhancedFileSystemWatcherTimeoutMs)
        {
            IncludeSubdirectories = includeSubdirectories
        };
        _enhancedFileSystemWatcher.Created += EnhancedFileSystemWatcherCreated;
        _enhancedFileSystemWatcher.Changed += EnhancedFileSystemWatcherChanged;
        _enhancedFileSystemWatcher.Deleted += EnhancedFileSystemWatcherDeleted;
        _enhancedFileSystemWatcher.EnableRaisingEvents = true;
    }

    /// <inheritdoc cref="IWireMockServer.WatchStaticMappings" />
    [PublicAPI]
    public bool ReadStaticMappingAndAddOrUpdate(string path)
    {
        Guard.NotNull(path);

        string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

        if (FileHelper.TryReadMappingFileWithRetryAndDelay(_settings.FileSystemHandler, path, out var value))
        {
            var mappingModels = DeserializeJsonToArray<MappingModel>(value);
            if (mappingModels.Length == 1 && Guid.TryParse(filenameWithoutExtension, out Guid guidFromFilename))
            {
                ConvertMappingAndRegisterAsRespondProvider(mappingModels[0], guidFromFilename, path);
            }
            else
            {
                ConvertMappingsAndRegisterAsRespondProvider(mappingModels, path);
            }

            return true;
        }

        return false;
    }
    #endregion

    #region Proxy and Record
    private HttpClient? _httpClientForProxy;

    private void InitProxyAndRecord(WireMockServerSettings settings)
    {
        if (settings.ProxyAndRecordSettings == null)
        {
            _httpClientForProxy = null;
            DeleteMapping(ProxyMappingGuid);
            return;
        }

        _httpClientForProxy = HttpClientBuilder.Build(settings.ProxyAndRecordSettings);

        var proxyRespondProvider = Given(Request.Create().WithPath("/*").UsingAnyMethod()).WithGuid(ProxyMappingGuid).WithTitle("Default Proxy Mapping on /*");
        if (settings.StartAdminInterface == true)
        {
            proxyRespondProvider.AtPriority(WireMockConstants.ProxyPriority);
        }

        proxyRespondProvider.RespondWith(new ProxyAsyncResponseProvider(ProxyAndRecordAsync, settings));
    }

    private async Task<IResponseMessage> ProxyAndRecordAsync(IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        var requestUri = new Uri(requestMessage.Url);
        var proxyUri = new Uri(settings.ProxyAndRecordSettings!.Url);
        var proxyUriWithRequestPathAndQuery = new Uri(proxyUri, requestUri.PathAndQuery);

        var proxyHelper = new ProxyHelper(settings);

        var (responseMessage, mapping) = await proxyHelper.SendAsync(
            _settings.ProxyAndRecordSettings!,
            _httpClientForProxy!,
            requestMessage,
            proxyUriWithRequestPathAndQuery.AbsoluteUri
        ).ConfigureAwait(false);

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
    private IResponseMessage SettingsGet(IRequestMessage requestMessage)
    {
        var model = new SettingsModel
        {
            AllowBodyForAllHttpMethods = _settings.AllowBodyForAllHttpMethods,
            AllowPartialMapping = _settings.AllowPartialMapping,
            GlobalProcessingDelay = (int?)_options.RequestProcessingDelay?.TotalMilliseconds,
            HandleRequestsSynchronously = _settings.HandleRequestsSynchronously,
            MaxRequestLogCount = _settings.MaxRequestLogCount,
            ReadStaticMappings = _settings.ReadStaticMappings,
            RequestLogExpirationDuration = _settings.RequestLogExpirationDuration,
            SaveUnmatchedRequests = _settings.SaveUnmatchedRequests,
            ThrowExceptionWhenMatcherFails = _settings.ThrowExceptionWhenMatcherFails,
            UseRegexExtended = _settings.UseRegexExtended,
            WatchStaticMappings = _settings.WatchStaticMappings,
            WatchStaticMappingsInSubdirectories = _settings.WatchStaticMappingsInSubdirectories,

#if USE_ASPNETCORE
            CorsPolicyOptions = _settings.CorsPolicyOptions?.ToString()
#endif
        };

        model.ProxyAndRecordSettings = TinyMapperUtils.Instance.Map(_settings.ProxyAndRecordSettings);

        return ToJson(model);
    }

    private IResponseMessage SettingsUpdate(IRequestMessage requestMessage)
    {
        var settings = DeserializeObject<SettingsModel>(requestMessage);

        // _settings
        _settings.AllowBodyForAllHttpMethods = settings.AllowBodyForAllHttpMethods;
        _settings.AllowPartialMapping = settings.AllowPartialMapping;
        _settings.HandleRequestsSynchronously = settings.HandleRequestsSynchronously;
        _settings.MaxRequestLogCount = settings.MaxRequestLogCount;
        _settings.ProxyAndRecordSettings = TinyMapperUtils.Instance.Map(settings.ProxyAndRecordSettings);
        _settings.ReadStaticMappings = settings.ReadStaticMappings;
        _settings.RequestLogExpirationDuration = settings.RequestLogExpirationDuration;
        _settings.SaveUnmatchedRequests = settings.SaveUnmatchedRequests;
        _settings.ThrowExceptionWhenMatcherFails = settings.ThrowExceptionWhenMatcherFails;
        _settings.UseRegexExtended = settings.UseRegexExtended;
        _settings.WatchStaticMappings = settings.WatchStaticMappings;
        _settings.WatchStaticMappingsInSubdirectories = settings.WatchStaticMappingsInSubdirectories;

        InitSettings(_settings);

        // _options
        if (settings.GlobalProcessingDelay != null)
        {
            _options.RequestProcessingDelay = TimeSpan.FromMilliseconds(settings.GlobalProcessingDelay.Value);
        }
        _options.AllowBodyForAllHttpMethods = settings.AllowBodyForAllHttpMethods;
        _options.AllowPartialMapping = settings.AllowPartialMapping;
        _options.HandleRequestsSynchronously = settings.HandleRequestsSynchronously;
        _options.MaxRequestLogCount = settings.MaxRequestLogCount;
        _options.RequestLogExpirationDuration = settings.RequestLogExpirationDuration;

        // _settings & _options
#if USE_ASPNETCORE
        if (Enum.TryParse<CorsPolicyOptions>(settings.CorsPolicyOptions, true, out var corsPolicyOptions))
        {
            _settings.CorsPolicyOptions = corsPolicyOptions;
            _options.CorsPolicyOptions = corsPolicyOptions;
        }
#endif

        return ResponseMessageBuilder.Create("Settings updated");
    }
    #endregion Settings

    #region Mapping/{guid}
    private IResponseMessage MappingGet(IRequestMessage requestMessage)
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

    private IResponseMessage MappingPut(IRequestMessage requestMessage)
    {
        Guid guid = ParseGuidFromRequestMessage(requestMessage);

        var mappingModel = DeserializeObject<MappingModel>(requestMessage);
        Guid? guidFromPut = ConvertMappingAndRegisterAsRespondProvider(mappingModel, guid);

        return ResponseMessageBuilder.Create("Mapping added or updated", HttpStatusCode.OK, guidFromPut);
    }

    private IResponseMessage MappingDelete(IRequestMessage requestMessage)
    {
        Guid guid = ParseGuidFromRequestMessage(requestMessage);

        if (DeleteMapping(guid))
        {
            return ResponseMessageBuilder.Create("Mapping removed", HttpStatusCode.OK, guid);
        }

        return ResponseMessageBuilder.Create("Mapping not found", HttpStatusCode.NotFound);
    }

    private static Guid ParseGuidFromRequestMessage(IRequestMessage requestMessage)
    {
        return Guid.Parse(requestMessage.Path.Substring(AdminMappings.Length + 1));
    }
    #endregion Mapping/{guid}

    #region Mappings
    private IResponseMessage SwaggerGet(IRequestMessage requestMessage)
    {
        return new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = SwaggerMapper.ToSwagger(this)
            },
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(WireMockConstants.ContentTypeJson) } }
        };
    }

    private IResponseMessage MappingsSave(IRequestMessage requestMessage)
    {
        SaveStaticMappings();

        return ResponseMessageBuilder.Create("Mappings saved to disk");
    }

    private IEnumerable<MappingModel> ToMappingModels()
    {
        return Mappings.Where(m => !m.IsAdminInterface).Select(_mappingConverter.ToMappingModel);
    }

    private IResponseMessage MappingsGet(IRequestMessage requestMessage)
    {
        return ToJson(ToMappingModels());
    }

    private IResponseMessage MappingsPost(IRequestMessage requestMessage)
    {
        try
        {
            var mappingModels = DeserializeRequestMessageToArray<MappingModel>(requestMessage);
            if (mappingModels.Length == 1)
            {
                Guid? guid = ConvertMappingAndRegisterAsRespondProvider(mappingModels[0]);
                return ResponseMessageBuilder.Create("Mapping added", 201, guid);
            }

            ConvertMappingsAndRegisterAsRespondProvider(mappingModels);

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

    private IResponseMessage MappingsDelete(IRequestMessage requestMessage)
    {
        if (!string.IsNullOrEmpty(requestMessage.Body))
        {
            var deletedGuids = MappingsDeleteMappingFromBody(requestMessage);
            if (deletedGuids != null)
            {
                return ResponseMessageBuilder.Create($"Mappings deleted. Affected GUIDs: [{string.Join(", ", deletedGuids.ToArray())}]");
            }

            // return bad request
            return ResponseMessageBuilder.Create("Poorly formed mapping JSON.", 400);
        }

        ResetMappings();

        ResetScenarios();

        return ResponseMessageBuilder.Create("Mappings deleted");
    }

    private IEnumerable<Guid>? MappingsDeleteMappingFromBody(IRequestMessage requestMessage)
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

    private IResponseMessage MappingsReset(IRequestMessage requestMessage)
    {
        ResetMappings();

        ResetScenarios();

        string message = "Mappings reset";
        if (requestMessage.Query != null &&
            requestMessage.Query.ContainsKey(QueryParamReloadStaticMappings) &&
            bool.TryParse(requestMessage.Query[QueryParamReloadStaticMappings].ToString(), out bool reloadStaticMappings) &&
            reloadStaticMappings)
        {
            ReadStaticMappings();
            message = $"{message} and static mappings reloaded";
        }

        return ResponseMessageBuilder.Create(message);
    }
    #endregion Mappings

    #region Request/{guid}
    private IResponseMessage RequestGet(IRequestMessage requestMessage)
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

    private IResponseMessage RequestDelete(IRequestMessage requestMessage)
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
    private IResponseMessage RequestsGet(IRequestMessage requestMessage)
    {
        var result = LogEntries
            .Where(r => !r.RequestMessage.Path.StartsWith("/__admin/"))
            .Select(LogEntryMapper.Map);

        return ToJson(result);
    }

    private IResponseMessage RequestsDelete(IRequestMessage requestMessage)
    {
        ResetLogEntries();

        return ResponseMessageBuilder.Create("Requests deleted");
    }
    #endregion Requests

    #region Requests/find
    private IResponseMessage RequestsFind(IRequestMessage requestMessage)
    {
        var requestModel = DeserializeObject<RequestModel>(requestMessage);

        var request = (Request)InitRequestBuilder(requestModel, false)!;

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
    private IResponseMessage ScenariosGet(IRequestMessage requestMessage)
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

    private IResponseMessage ScenariosReset(IRequestMessage requestMessage)
    {
        ResetScenarios();

        return ResponseMessageBuilder.Create("Scenarios reset");
    }
    #endregion

    #region Pact
    /// <summary>
    /// Save the mappings as a Pact Json file V2.
    /// </summary>
    /// <param name="folder">The folder to save the pact file.</param>
    /// <param name="filename">The filename for the .json file [optional].</param>
    [PublicAPI]
    public void SavePact(string folder, string? filename = null)
    {
        var (filenameUpdated, bytes) = PactMapper.ToPact(this, filename);
        _settings.FileSystemHandler.WriteFile(folder, filenameUpdated, bytes);
    }

    /// <summary>
    /// Save the mappings as a Pact Json file V2.
    /// </summary>
    /// <param name="stream">The (file) stream.</param>
    [PublicAPI]
    public void SavePact(Stream stream)
    {
        var (_, bytes) = PactMapper.ToPact(this);
        using var writer = new BinaryWriter(stream);
        writer.Write(bytes);

        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    /// <summary>
    /// This stores details about the consumer of the interaction.
    /// </summary>
    /// <param name="consumer">the consumer</param>
    [PublicAPI]
    public WireMockServer WithConsumer(string consumer)
    {
        Consumer = consumer;
        return this;
    }

    /// <summary>
    /// This stores details about the provider of the interaction.
    /// </summary>
    /// <param name="provider">the provider</param>
    [PublicAPI]
    public WireMockServer WithProvider(string provider)
    {
        Provider = provider;
        return this;
    }
    #endregion

    private void DisposeEnhancedFileSystemWatcher()
    {
        if (_enhancedFileSystemWatcher != null)
        {
            _enhancedFileSystemWatcher.EnableRaisingEvents = false;

            _enhancedFileSystemWatcher.Created -= EnhancedFileSystemWatcherCreated;
            _enhancedFileSystemWatcher.Changed -= EnhancedFileSystemWatcherChanged;
            _enhancedFileSystemWatcher.Deleted -= EnhancedFileSystemWatcherDeleted;

            _enhancedFileSystemWatcher.Dispose();
        }
    }

    private void EnhancedFileSystemWatcherCreated(object sender, FileSystemEventArgs args)
    {
        _settings.Logger.Info("MappingFile created : '{0}', reading file.", args.FullPath);
        if (!ReadStaticMappingAndAddOrUpdate(args.FullPath))
        {
            _settings.Logger.Error("Unable to read MappingFile '{0}'.", args.FullPath);
        }
    }

    private void EnhancedFileSystemWatcherChanged(object sender, FileSystemEventArgs args)
    {
        _settings.Logger.Info("MappingFile updated : '{0}', reading file.", args.FullPath);
        if (!ReadStaticMappingAndAddOrUpdate(args.FullPath))
        {
            _settings.Logger.Error("Unable to read MappingFile '{0}'.", args.FullPath);
        }
    }

    private void EnhancedFileSystemWatcherDeleted(object sender, FileSystemEventArgs args)
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
    }

    private static Encoding? ToEncoding(EncodingModel? encodingModel)
    {
        return encodingModel != null ? Encoding.GetEncoding(encodingModel.CodePage) : null;
    }

    private static ResponseMessage ToJson<T>(T result, bool keepNullValues = false)
    {
        return new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = JsonConvert.SerializeObject(result, keepNullValues ? JsonSerializationConstants.JsonSerializerSettingsIncludeNullValues : JsonSerializationConstants.JsonSerializerSettingsDefault)
            },
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(WireMockConstants.ContentTypeJson) } }
        };
    }

    private static T DeserializeObject<T>(IRequestMessage requestMessage) where T : new()
    {
        return requestMessage.BodyData?.DetectedBodyType switch
        {
            BodyType.String => JsonUtils.DeserializeObject<T>(requestMessage.BodyData.BodyAsString),

            BodyType.Json when requestMessage.BodyData?.BodyAsJson != null => ((JObject)requestMessage.BodyData.BodyAsJson).ToObject<T>()!,

            _ => throw new NotSupportedException()
        };
    }

    private static T[] DeserializeRequestMessageToArray<T>(IRequestMessage requestMessage)
    {
        if (requestMessage.BodyData?.DetectedBodyType == BodyType.Json && requestMessage.BodyData.BodyAsJson != null)
        {
            var bodyAsJson = requestMessage.BodyData.BodyAsJson;

            return DeserializeObjectToArray<T>(bodyAsJson);
        }

        throw new NotSupportedException();
    }

    private static T[] DeserializeJsonToArray<T>(string value)
    {
        return DeserializeObjectToArray<T>(JsonUtils.DeserializeObject(value));
    }

    private static T[] DeserializeObjectToArray<T>(object value)
    {
        if (value is JArray jArray)
        {
            return jArray.ToObject<T[]>();
        }

        var singleResult = ((JObject)value).ToObject<T>();
        return new[] { singleResult! };
    }
}
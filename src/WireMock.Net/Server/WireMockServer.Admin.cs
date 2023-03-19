using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
using WireMock.RequestBuilders;
using WireMock.ResponseProviders;
using WireMock.Serialization;
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
    private const string AdminMappingsCode = "/__admin/mappings/code";
    private const string AdminMappingsWireMockOrg = "/__admin/mappings/wiremock.org";
    private const string AdminRequests = "/__admin/requests";
    private const string AdminSettings = "/__admin/settings";
    private const string AdminScenarios = "/__admin/scenarios";
    private const string QueryParamReloadStaticMappings = "reloadStaticMappings";

    private static readonly Guid ProxyMappingGuid = new("e59914fd-782e-428e-91c1-4810ffb86567");
    private static readonly RegexMatcher AdminRequestContentTypeJson = new ContentTypeMatcher(WireMockConstants.ContentTypeJson, true);
    private static readonly RegexMatcher AdminMappingsGuidPathMatcher = new(@"^\/__admin\/mappings\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");
    private static readonly RegexMatcher AdminMappingsCodeGuidPathMatcher = new(@"^\/__admin\/mappings\/code\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");
    private static readonly RegexMatcher AdminRequestsGuidPathMatcher = new(@"^\/__admin\/requests\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");
    private static readonly RegexMatcher AdminScenariosNameMatcher = new(@"^\/__admin\/scenarios\/.+$");
    private static readonly RegexMatcher AdminScenariosNameWithResetMatcher = new(@"^\/__admin\/scenarios\/.+\/reset$");

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
        Given(Request.Create().WithPath(AdminMappings).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsDelete));

        // __admin/mappings/code
        Given(Request.Create().WithPath(AdminMappingsCode).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsCodeGet));

        // __admin/mappings/wiremock.org
        Given(Request.Create().WithPath(AdminMappingsWireMockOrg).UsingPost().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsPostWireMockOrg));

        // __admin/mappings/reset
        Given(Request.Create().WithPath(AdminMappings + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsReset));

        // __admin/mappings/{guid}
        Given(Request.Create().WithPath(AdminMappingsGuidPathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingGet));
        Given(Request.Create().WithPath(AdminMappingsGuidPathMatcher).UsingPut().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingPut));
        Given(Request.Create().WithPath(AdminMappingsGuidPathMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingDelete));

        // __admin/mappings/code/{guid}
        Given(Request.Create().WithPath(AdminMappingsCodeGuidPathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingCodeGet));

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
        Given(Request.Create().WithPath(AdminScenariosNameMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenarioReset));

        // __admin/scenarios/reset
        Given(Request.Create().WithPath(AdminScenarios + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosReset));
        Given(Request.Create().WithPath(AdminScenariosNameWithResetMatcher).UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenarioReset));

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
        _mappingBuilder.SaveMappingsToFolder(folder);
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

    #region Settings
    private IResponseMessage SettingsGet(IRequestMessage requestMessage)
    {
        var model = new SettingsModel
        {
            AllowBodyForAllHttpMethods = _settings.AllowBodyForAllHttpMethods,
            AllowOnlyDefinedHttpStatusCodeInResponse = _settings.AllowOnlyDefinedHttpStatusCodeInResponse,
            AllowPartialMapping = _settings.AllowPartialMapping,
            DisableDeserializeFormUrlEncoded = _settings.DisableDeserializeFormUrlEncoded,
            DisableJsonBodyParsing = _settings.DisableJsonBodyParsing,
            DisableRequestBodyDecompressing = _settings.DisableRequestBodyDecompressing,
            DoNotSaveDynamicResponseInLogEntry = _settings.DoNotSaveDynamicResponseInLogEntry,
            GlobalProcessingDelay = (int?)_options.RequestProcessingDelay?.TotalMilliseconds,
            HandleRequestsSynchronously = _settings.HandleRequestsSynchronously,
            HostingScheme = _settings.HostingScheme,
            MaxRequestLogCount = _settings.MaxRequestLogCount,
            QueryParameterMultipleValueSupport = _settings.QueryParameterMultipleValueSupport,
            ReadStaticMappings = _settings.ReadStaticMappings,
            RequestLogExpirationDuration = _settings.RequestLogExpirationDuration,
            SaveUnmatchedRequests = _settings.SaveUnmatchedRequests,
            ThrowExceptionWhenMatcherFails = _settings.ThrowExceptionWhenMatcherFails,
            UseRegexExtended = _settings.UseRegexExtended,
            WatchStaticMappings = _settings.WatchStaticMappings,
            WatchStaticMappingsInSubdirectories = _settings.WatchStaticMappingsInSubdirectories,

#if USE_ASPNETCORE
            AcceptAnyClientCertificate = _settings.AcceptAnyClientCertificate,
            ClientCertificateMode = _settings.ClientCertificateMode,
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
        _settings.AllowOnlyDefinedHttpStatusCodeInResponse = settings.AllowOnlyDefinedHttpStatusCodeInResponse;
        _settings.AllowPartialMapping = settings.AllowPartialMapping;
        _settings.DisableDeserializeFormUrlEncoded = settings.DisableDeserializeFormUrlEncoded;
        _settings.DisableJsonBodyParsing = settings.DisableJsonBodyParsing;
        _settings.DisableRequestBodyDecompressing = settings.DisableRequestBodyDecompressing;
        _settings.DoNotSaveDynamicResponseInLogEntry = settings.DoNotSaveDynamicResponseInLogEntry;
        _settings.HandleRequestsSynchronously = settings.HandleRequestsSynchronously;
        _settings.MaxRequestLogCount = settings.MaxRequestLogCount;
        _settings.ProxyAndRecordSettings = TinyMapperUtils.Instance.Map(settings.ProxyAndRecordSettings);
        _settings.QueryParameterMultipleValueSupport = settings.QueryParameterMultipleValueSupport;
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

        _options.ClientCertificateMode = _settings.ClientCertificateMode;
        _options.AcceptAnyClientCertificate = _settings.AcceptAnyClientCertificate;
#endif

        return ResponseMessageBuilder.Create("Settings updated");
    }
    #endregion Settings

    #region Mapping/{guid}
    private IResponseMessage MappingGet(IRequestMessage requestMessage)
    {
        var mapping = FindMappingByGuid(requestMessage);
        if (mapping == null)
        {
            _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
            return ResponseMessageBuilder.Create("Mapping not found", HttpStatusCode.NotFound);
        }

        var model = _mappingConverter.ToMappingModel(mapping);

        return ToJson(model);
    }

    private IResponseMessage MappingCodeGet(IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid))
        {
            var code = _mappingBuilder.ToCSharpCode(guid, GetMappingConverterType(requestMessage));
            if (code is null)
            {
                _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
                return ResponseMessageBuilder.Create("Mapping not found", HttpStatusCode.NotFound);
            }

            return ToResponseMessage(code);
        }

        _settings.Logger.Warn("HttpStatusCode set to 400");
        return ResponseMessageBuilder.Create("GUID is missing", HttpStatusCode.BadRequest);
    }

    private static MappingConverterType GetMappingConverterType(IRequestMessage requestMessage)
    {
        var mappingConverterType = MappingConverterType.Server;

        if (requestMessage.QueryIgnoreCase?.TryGetValue(nameof(MappingConverterType), out var values) == true &&
            Enum.TryParse(values.FirstOrDefault(), true, out MappingConverterType parsed))
        {
            mappingConverterType = parsed;
        }

        return mappingConverterType;
    }

    private IMapping? FindMappingByGuid(IRequestMessage requestMessage)
    {
        return TryParseGuidFromRequestMessage(requestMessage, out var guid) ? Mappings.FirstOrDefault(m => !m.IsAdminInterface && m.Guid == guid) : null;
    }

    private IResponseMessage MappingPut(IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid))
        {
            var mappingModel = DeserializeObject<MappingModel>(requestMessage);
            var guidFromPut = ConvertMappingAndRegisterAsRespondProvider(mappingModel, guid);

            return ResponseMessageBuilder.Create("Mapping added or updated", HttpStatusCode.OK, guidFromPut);
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
        return ResponseMessageBuilder.Create("Mapping not found", HttpStatusCode.NotFound);
    }

    private IResponseMessage MappingDelete(IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid) && DeleteMapping(guid))
        {
            return ResponseMessageBuilder.Create("Mapping removed", HttpStatusCode.OK, guid);
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
        return ResponseMessageBuilder.Create("Mapping not found", HttpStatusCode.NotFound);
    }

    private static bool TryParseGuidFromRequestMessage(IRequestMessage requestMessage, out Guid guid)
    {
        var lastPart = requestMessage.Path.Split('/').LastOrDefault();
        return Guid.TryParse(lastPart, out guid);
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

    private MappingModel[] ToMappingModels()
    {
        return _mappingBuilder.GetMappings();
    }

    private IResponseMessage MappingsGet(IRequestMessage requestMessage)
    {
        return ToJson(ToMappingModels());
    }

    private IResponseMessage MappingsCodeGet(IRequestMessage requestMessage)
    {
        var converterType = GetMappingConverterType(requestMessage);

        var code = _mappingBuilder.ToCSharpCode(converterType);

        return ToResponseMessage(code);
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
            foreach (var guid in mappingModels.Where(mm => mm.Guid.HasValue).Select(mm => mm.Guid!.Value))
            {
                if (DeleteMapping(guid))
                {
                    deletedGuids.Add(guid);
                }
                else
                {
                    _settings.Logger.Debug($"Did not find/delete mapping with GUID: {guid}.");
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
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid))
        {
            var entry = LogEntries.FirstOrDefault(r => !r.RequestMessage.Path.StartsWith("/__admin/") && r.Guid == guid);
            if (entry is { })
            {
                var model = new LogEntryMapper(_options).Map(entry);
                return ToJson(model);
            }
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Request not found");
        return ResponseMessageBuilder.Create("Request not found", HttpStatusCode.NotFound);
    }

    private IResponseMessage RequestDelete(IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid) && DeleteLogEntry(guid))
        {
            return ResponseMessageBuilder.Create("Request removed");
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Request not found");
        return ResponseMessageBuilder.Create("Request not found", HttpStatusCode.NotFound);
    }
    #endregion Request/{guid}

    #region Requests
    private IResponseMessage RequestsGet(IRequestMessage requestMessage)
    {
        var logEntryMapper = new LogEntryMapper(_options);
        var result = LogEntries
            .Where(r => !r.RequestMessage.Path.StartsWith("/__admin/"))
            .Select(logEntryMapper.Map);

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

        var logEntryMapper = new LogEntryMapper(_options);
        var result = dict.OrderBy(x => x.Value.AverageTotalScore).Select(x => x.Key).Select(logEntryMapper.Map);

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

    private IResponseMessage ScenarioReset(IRequestMessage requestMessage)
    {
        var name = string.Equals(HttpRequestMethod.DELETE, requestMessage.Method, StringComparison.OrdinalIgnoreCase) ?
            requestMessage.Path.Substring(AdminScenarios.Length + 1) :
            requestMessage.Path.Split('/').Reverse().Skip(1).First();

        return ResetScenario(name) ?
            ResponseMessageBuilder.Create("Scenario reset") :
            ResponseMessageBuilder.Create($"No scenario found by name '{name}'.", HttpStatusCode.NotFound);
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

    private static ResponseMessage ToResponseMessage(string text)
    {
        return new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = text
            },
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(WireMockConstants.ContentTypeTextPlain) } }
        };
    }

    private static T DeserializeObject<T>(IRequestMessage requestMessage) where T : new()
    {
        switch (requestMessage.BodyData?.DetectedBodyType)
        {
            case BodyType.String:
            case BodyType.FormUrlEncoded:
                return JsonUtils.DeserializeObject<T>(requestMessage.BodyData.BodyAsString!);

            case BodyType.Json when requestMessage.BodyData?.BodyAsJson != null:
                return ((JObject)requestMessage.BodyData.BodyAsJson).ToObject<T>()!;

            default:
                throw new NotSupportedException();
        }
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
            return jArray.ToObject<T[]>()!;
        }

        var singleResult = ((JObject)value).ToObject<T>();
        return new[] { singleResult! };
    }
}
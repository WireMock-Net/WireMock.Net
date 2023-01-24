using System.Linq;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Matchers.Request;
using WireMock.Owin;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock;

/// <summary>
/// MappingBuilder
/// </summary>
public class MappingBuilder : IMappingBuilder
{
    private readonly WireMockServerSettings _settings;
    private readonly IWireMockMiddlewareOptions _options;
    private readonly MappingConverter _mappingConverter;
    private readonly MappingToFileSaver _mappingToFileSaver;
    private readonly IGuidUtils _guidUtils;
    private readonly IDateTimeUtils _dateTimeUtils;

    /// <summary>
    /// Create a MappingBuilder
    /// </summary>
    /// <param name="settings">The optional <see cref="WireMockServerSettings"/>.</param>
    public MappingBuilder(WireMockServerSettings? settings = null)
    {
        _settings = settings ?? new WireMockServerSettings();
        _options = WireMockMiddlewareOptionsHelper.InitFromSettings(_settings);

        var matcherMapper = new MatcherMapper(_settings);
        _mappingConverter = new MappingConverter(matcherMapper);
        _mappingToFileSaver = new MappingToFileSaver(_settings, _mappingConverter);

        _guidUtils = new GuidUtils();
        _dateTimeUtils = new DateTimeUtils();
    }

    internal MappingBuilder(
        WireMockServerSettings settings,
        IWireMockMiddlewareOptions options,
        MappingConverter mappingConverter,
        MappingToFileSaver mappingToFileSaver,
        IGuidUtils guidUtils,
        IDateTimeUtils dateTimeUtils
    )
    {
        _settings = Guard.NotNull(settings);
        _options = Guard.NotNull(options);
        _mappingConverter = Guard.NotNull(mappingConverter);
        _mappingToFileSaver = Guard.NotNull(mappingToFileSaver);
        _guidUtils = Guard.NotNull(guidUtils);
        _dateTimeUtils = Guard.NotNull(dateTimeUtils);
    }

    /// <inheritdoc />
    public IRespondWithAProvider Given(IRequestMatcher requestMatcher, bool saveToFile = false)
    {
        return new RespondWithAProvider(RegisterMapping, Guard.NotNull(requestMatcher), _settings, _guidUtils, _dateTimeUtils, saveToFile);
    }

    /// <inheritdoc />
    public MappingModel[] GetMappings()
    {
        return GetMappingsInternal().Select(_mappingConverter.ToMappingModel).ToArray();
    }

    internal IMapping[] GetMappingsInternal()
    {
        return _options.Mappings.Values.ToArray().Where(m => !m.IsAdminInterface).ToArray();
    }

    /// <inheritdoc />
    public string ToJson()
    {
        return ToJson(GetMappings());
    }

    /// <inheritdoc />
    public void SaveMappingsToFile(string path)
    {
        _mappingToFileSaver.SaveMappingsToFile(GetNonAdminMappings(), path);
    }

    /// <inheritdoc />
    public void SaveMappingsToFolder(string? folder)
    {
        foreach (var mapping in GetNonAdminMappings().Where(m => !m.IsAdminInterface))
        {
            _mappingToFileSaver.SaveMappingToFile(mapping, folder);
        }
    }

    private IMapping[] GetNonAdminMappings()
    {
        return _options.Mappings.Values.ToArray();
    }

    private void RegisterMapping(IMapping mapping, bool saveToFile)
    {
        // Check a mapping exists with the same Guid. If so, update the datetime and replace it.
        if (_options.Mappings.ContainsKey(mapping.Guid))
        {
            mapping.UpdatedAt = _dateTimeUtils.UtcNow;
            _options.Mappings[mapping.Guid] = mapping;
        }
        else
        {
            _options.Mappings.TryAdd(mapping.Guid, mapping);
        }

        if (saveToFile)
        {
            _mappingToFileSaver.SaveMappingToFile(mapping);
        }
    }

    private static string ToJson(object value)
    {
        return JsonConvert.SerializeObject(value, JsonSerializationConstants.JsonSerializerSettingsDefault);
    }
}
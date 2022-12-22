using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Matchers.Request;
using WireMock.Owin;
using WireMock.Serialization;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock;

/// <summary>
/// MappingBuilder
/// </summary>
public class MappingBuilder : IMappingBuilder
{
    private readonly IWireMockMiddlewareOptions _options;
    private readonly WireMockServerSettings _settings;

    private readonly MappingConverter _mappingConverter;

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
    }

    /// <inheritdoc />
    public IRespondWithAProvider Given(IRequestMatcher requestMatcher)
    {
        return new RespondWithAProvider(RegisterMapping, Guard.NotNull(requestMatcher), _settings);
    }

    /// <inheritdoc />
    public MappingModel[] GetMappings()
    {
        return _options.Mappings.Values.ToArray()
            .Where(m => !m.IsAdminInterface)
            .Select(_mappingConverter.ToMappingModel)
            .ToArray();
    }

    /// <inheritdoc />
    public string ToJson()
    {
        return ToJson(_options.Mappings.Values.ToArray());
    }

    /// <inheritdoc />
    public void SaveMappingsToFile(string path)
    {
        File.WriteAllText(Guard.NotNullOrEmpty(path), ToJson());
    }

    /// <inheritdoc />
    public void SaveMappingsToFiles(string folder)
    {
        Guard.NotNullOrEmpty(folder);

        foreach (var mapping in GetMappings())
        {
            File.WriteAllText(Path.Combine(folder, mapping.Guid.ToString()), ToJson(mapping));
        }
    }

    private void RegisterMapping(IMapping mapping, bool _)
    {
        // Check a mapping exists with the same Guid. If so, update the datetime and replace it.
        if (_options.Mappings.ContainsKey(mapping.Guid))
        {
            mapping.UpdatedAt = DateTime.UtcNow;
            _options.Mappings[mapping.Guid] = mapping;
        }
        else
        {
            _options.Mappings.TryAdd(mapping.Guid, mapping);
        }
    }

    private static string ToJson(object value)
    {
        return JsonConvert.SerializeObject(value, JsonSerializationConstants.JsonSerializerSettingsDefault);
    }
}
using System.IO;
using System.Linq;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.Serialization;

/// <summary>
/// Creates sanitized file names for mappings
/// </summary>
public class MappingFileNameSanitizer
{
    private readonly char _replaceChar = '_';

    private readonly WireMockServerSettings _settings;

    public MappingFileNameSanitizer(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
    }

    /// <summary>
    /// Creates sanitized file names for mappings
    /// </summary>
    public string BuildSanitizedFileName(IMapping mapping)
    {
        const string mappingTitlePrefix = "Proxy Mapping for ";
        string name;
        if (!string.IsNullOrEmpty(mapping.Title))
        {
            // remove 'Proxy Mapping for ' and an extra space character after the HTTP request method
            name = mapping.Title.Replace(mappingTitlePrefix, "").Replace(' '.ToString(), string.Empty);
            if (_settings.ProxyAndRecordSettings?.AppendGuidToSavedMappingFile == true)
            {
                name += $"{_replaceChar}{mapping.Guid}";
            }
        }
        else
        {
            name = mapping.Guid.ToString();
        }

        if (!string.IsNullOrEmpty(_settings.ProxyAndRecordSettings?.PrefixForSavedMappingFile))
        {
            name = $"{_settings.ProxyAndRecordSettings.PrefixForSavedMappingFile}{_replaceChar}{name}";
        }
        return $"{Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, _replaceChar))}.json";
    }
}

// Copyright © WireMock.Net

using System.IO;
using System.Linq;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.Serialization;

/// <summary>
/// Creates sanitized file names for mappings
/// </summary>
internal class MappingFileNameSanitizer
{
    private const string SpaceChar = " ";
    private const char ReplaceChar = '_';

    /// <summary>
    /// Creates sanitized file names for mappings
    /// </summary>
    public string BuildSanitizedFileName(IMapping mapping, ProxyAndRecordSettings? proxyAndRecordSettings)
    {
        string name;
        if (!string.IsNullOrEmpty(mapping.Title))
        {
            // remove 'Proxy Mapping for ' and an extra space character after the HTTP request method
            name = mapping.Title!.Replace(ProxyAndRecordSettings.DefaultPrefixForSavedMappingFile, string.Empty).Replace(SpaceChar, string.Empty);
            if (proxyAndRecordSettings?.AppendGuidToSavedMappingFile == true)
            {
                name += $"{ReplaceChar}{mapping.Guid}";
            }
        }
        else
        {
            name = mapping.Guid.ToString();
        }

        if (!string.IsNullOrEmpty(proxyAndRecordSettings?.PrefixForSavedMappingFile))
        {
            name = $"{proxyAndRecordSettings.PrefixForSavedMappingFile}{ReplaceChar}{name}";
        }

        return $"{Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, ReplaceChar))}.json";
    }
}

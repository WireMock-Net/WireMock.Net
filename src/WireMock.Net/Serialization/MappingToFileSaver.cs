using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.Serialization;

internal class MappingToFileSaver
{
    private readonly WireMockServerSettings _settings;
    private readonly MappingConverter _mappingConverter;

    public MappingToFileSaver(WireMockServerSettings settings, MappingConverter mappingConverter)
    {
        _settings = Guard.NotNull(settings);
        _mappingConverter = Guard.NotNull(mappingConverter);
    }

    public void SaveMappingToFile(IMapping mapping, string? folder = null)
    {
        folder ??= _settings.FileSystemHandler.GetMappingFolder();

        if (!_settings.FileSystemHandler.FolderExists(folder))
        {
            _settings.FileSystemHandler.CreateFolder(folder);
        }

        var model = _mappingConverter.ToMappingModel(mapping);

        var filename = BuildSanitizedFileName(mapping);
        var path = Path.Combine(folder, filename);

        _settings.Logger.Info("Saving Mapping file {0}", path);

        _settings.FileSystemHandler.WriteMappingFile(path, JsonConvert.SerializeObject(model, JsonSerializationConstants.JsonSerializerSettingsDefault));
    }

    private string BuildSanitizedFileName(IMapping mapping, char replaceChar = '_')
    {
        string name;
        if (!string.IsNullOrEmpty(mapping.Title))
        {
            name = mapping.Title!;
            if (_settings.ProxyAndRecordSettings?.AppendGuidToSavedMappingFile == true)
            {
                name += $"{replaceChar}{mapping.Guid}";
            }
        }
        else
        {
            name = mapping.Guid.ToString();
        }

        return $"{Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, replaceChar))}.json";
    }
}
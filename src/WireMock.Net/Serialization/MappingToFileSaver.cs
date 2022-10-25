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
        Guard.NotNull(settings);
        Guard.NotNull(mappingConverter);

        _settings = settings;
        _mappingConverter = mappingConverter;
    }

    public void SaveMappingToFile(IMapping mapping, string? folder = null)
    {
        folder ??= _settings.FileSystemHandler.GetMappingFolder();

        if (!_settings.FileSystemHandler.FolderExists(folder))
        {
            _settings.FileSystemHandler.CreateFolder(folder);
        }

        var model = _mappingConverter.ToMappingModel(mapping);
        string filename = (!string.IsNullOrEmpty(mapping.Title) ? SanitizeFileName(mapping.Title!) : mapping.Guid.ToString()) + ".json";

        string path = Path.Combine(folder, filename);

        _settings.Logger.Info("Saving Mapping file {0}", filename);

        _settings.FileSystemHandler.WriteMappingFile(path, JsonConvert.SerializeObject(model, JsonSerializationConstants.JsonSerializerSettingsDefault));
    }

    private static string SanitizeFileName(string name, char replaceChar = '_')
    {
        return Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, replaceChar));
    }
}
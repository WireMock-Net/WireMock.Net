// Copyright © WireMock.Net

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
    private readonly MappingFileNameSanitizer _fileNameSanitizer;

    public MappingToFileSaver(WireMockServerSettings settings, MappingConverter mappingConverter)
    {
        _settings = Guard.NotNull(settings);
        _mappingConverter = Guard.NotNull(mappingConverter);
        _fileNameSanitizer = new MappingFileNameSanitizer(settings);
    }

    public void SaveMappingsToFile(IMapping[] mappings, string? folder = null)
    {
        folder ??= _settings.FileSystemHandler.GetMappingFolder();

        if (!_settings.FileSystemHandler.FolderExists(folder))
        {
            _settings.FileSystemHandler.CreateFolder(folder);
        }

        var models = mappings.Select(_mappingConverter.ToMappingModel).ToArray();

        Save(models, folder);
    }

    public void SaveMappingToFile(IMapping mapping, string? folder = null)
    {
        folder ??= _settings.FileSystemHandler.GetMappingFolder();

        if (!_settings.FileSystemHandler.FolderExists(folder))
        {
            _settings.FileSystemHandler.CreateFolder(folder);
        }

        var model = _mappingConverter.ToMappingModel(mapping);

        var filename = _fileNameSanitizer.BuildSanitizedFileName(mapping);
        var path = Path.Combine(folder, filename);

        Save(model, path);
    }

    private void Save(object value, string path)
    {
        _settings.Logger.Info("Saving Mapping file {0}", path);

        _settings.FileSystemHandler.WriteMappingFile(path, JsonConvert.SerializeObject(value, JsonSerializationConstants.JsonSerializerSettingsDefault));
    }
}
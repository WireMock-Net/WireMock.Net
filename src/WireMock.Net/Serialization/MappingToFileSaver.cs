using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WireMock.Settings;
using Stef.Validation;

namespace WireMock.Serialization
{
    internal class MappingToFileSaver
    {
        private readonly IWireMockServerSettings _settings;
        private readonly MappingConverter _mappingConverter;

        public MappingToFileSaver(IWireMockServerSettings settings, MappingConverter mappingConverter)
        {
            Guard.NotNull(settings, nameof(settings));

            _settings = settings;
            _mappingConverter = mappingConverter;
        }

        public void SaveMappingToFile(IMapping mapping, string folder = null)
        {
            if (folder == null)
            {
                folder = _settings.FileSystemHandler.GetMappingFolder();
            }

            if (!_settings.FileSystemHandler.FolderExists(folder))
            {
                _settings.FileSystemHandler.CreateFolder(folder);
            }

            var model = _mappingConverter.ToMappingModel(mapping);
            string filename = (!string.IsNullOrEmpty(mapping.Title) ? SanitizeFileName(mapping.Title) : mapping.Guid.ToString()) + ".json";

            string path = Path.Combine(folder, filename);

            _settings.Logger.Info("Saving Mapping file {0}", filename);

            _settings.FileSystemHandler.WriteMappingFile(path, JsonConvert.SerializeObject(model, JsonSerializationConstants.JsonSerializerSettingsDefault));
        }

        private static string SanitizeFileName(string name, char replaceChar = '_')
        {
            return Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, replaceChar));
        }
    }
}
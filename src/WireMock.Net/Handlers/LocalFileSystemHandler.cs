using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Handlers
{
    /// <summary>
    /// Default implementation for a handler to interact with the local file system to read and write static mapping files.
    /// </summary>
    public class LocalFileSystemHandler : IFileSystemHandler
    {
        private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");

        /// <inheritdoc cref="IFileSystemHandler.FolderExists"/>
        public bool FolderExists([NotNull] string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            return Directory.Exists(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.CreateFolder"/>
        public void CreateFolder([NotNull] string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            Directory.CreateDirectory(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.EnumerateFiles"/>
        public IEnumerable<string> EnumerateFiles([NotNull] string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            return Directory.EnumerateFiles(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.GetMappingFolder"/>
        public string GetMappingFolder()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), AdminMappingsFolder);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadMappingFile"/>
        public string ReadMappingFile([NotNull] string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            return File.ReadAllText(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteMappingFile"/>
        public void WriteMappingFile([NotNull] string path, [NotNull] string text)
        {
            Check.NotNullOrEmpty(path, nameof(path));
            Check.NotNull(text, nameof(text));

            File.WriteAllText(path, text);
        }
    }
}

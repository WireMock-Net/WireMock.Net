using System.Collections.Generic;
using System.IO;

namespace WireMock.Handlers
{
    /// <summary>
    /// Default implementation for a handler to interact with the local file system to read and write static mapping files.
    /// </summary>
    public class LocalFileSystemHandler : IFileSystemHandler
    {
        private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");

        /// <inheritdoc cref="IFileSystemHandler.FolderExists"/>
        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.CreateFolder"/>
        public void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.EnumerateFiles"/>
        public IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.GetMappingFolder"/>
        public string GetMappingFolder()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), AdminMappingsFolder);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadMappingFile"/>
        public string ReadMappingFile(string path)
        {
            return File.ReadAllText(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteMappingFile"/>
        public void WriteMappingFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }
}

using System.Collections.Generic;
using System.IO;
using WireMock.Handlers;

namespace WireMock.Net.ConsoleApplication
{
    internal class CustomFileSystemFileHandler : IFileSystemHandler
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
            return Path.Combine(@"c:\temp-wiremock", AdminMappingsFolder);
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
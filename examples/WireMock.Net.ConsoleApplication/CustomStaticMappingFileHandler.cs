using System;
using System.Collections.Generic;
using System.IO;
using WireMock.Handlers;

namespace WireMock.Net.ConsoleApplication
{
    internal class CustomStaticMappingFileHandler : IStaticMappingHandler
    {
        private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");

        /// <inheritdoc cref="IStaticMappingHandler.FolderExists"/>
        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc cref="IStaticMappingHandler.CreateFolder"/>
        public void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc cref="IStaticMappingHandler.EnumerateFiles"/>
        public IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        /// <inheritdoc cref="IStaticMappingHandler.GetMappingFolder"/>
        public string GetMappingFolder()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), AdminMappingsFolder);
        }

        /// <inheritdoc cref="IStaticMappingHandler.ReadMappingFile"/>
        public Func<string, string> ReadMappingFile { get; } = (path) =>
        {
            return File.ReadAllText(path);
        };

        /// <inheritdoc cref="IStaticMappingHandler.WriteMappingFile"/>
        public Action<string, string> WriteMappingFile { get; } = (filename, text) =>
        {
            File.WriteAllText(filename, text);
        };
    }
}
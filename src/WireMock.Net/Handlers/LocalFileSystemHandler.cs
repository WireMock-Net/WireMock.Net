using System.Collections.Generic;
using System.IO;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Handlers
{
    /// <summary>
    /// Default implementation for a handler to interact with the local file system to read and write static mapping files.
    /// </summary>
    public class LocalFileSystemHandler : IFileSystemHandler
    {
        private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");
        private static readonly string UnmatchedRequestsFolder = Path.Combine("requests", "unmatched");

        private readonly string _rootFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileSystemHandler"/> class.
        /// </summary>
        public LocalFileSystemHandler() : this(Directory.GetCurrentDirectory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileSystemHandler"/> class.
        /// </summary>
        /// <param name="rootFolder">The root folder.</param>
        public LocalFileSystemHandler(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        /// <inheritdoc cref="IFileSystemHandler.FolderExists"/>
        public bool FolderExists(string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            return Directory.Exists(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.CreateFolder"/>
        public void CreateFolder(string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            Directory.CreateDirectory(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.EnumerateFiles"/>
        public IEnumerable<string> EnumerateFiles(string path, bool includeSubdirectories)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            return includeSubdirectories ? Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories) : Directory.EnumerateFiles(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.GetMappingFolder"/>
        public string GetMappingFolder()
        {
            return Path.Combine(_rootFolder, AdminMappingsFolder);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadMappingFile"/>
        public string ReadMappingFile(string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));

            return File.ReadAllText(path);
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteMappingFile(string, string)"/>
        public void WriteMappingFile(string path, string text)
        {
            Check.NotNullOrEmpty(path, nameof(path));
            Check.NotNull(text, nameof(text));

            File.WriteAllText(path, text);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadResponseBodyAsFile"/>
        public byte[] ReadResponseBodyAsFile(string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));
            path = PathUtils.CleanPath(path);
            // If the file exists at the given path relative to the MappingsFolder, then return that.
            // Else the path will just be as-is.
            return File.ReadAllBytes(File.Exists(PathUtils.Combine(GetMappingFolder(), path)) ? PathUtils.Combine(GetMappingFolder(), path) : path);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadResponseBodyAsString"/>
        public string ReadResponseBodyAsString(string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));
            path = PathUtils.CleanPath(path);
            // In case the path is a filename, the path will be adjusted to the MappingFolder.
            // Else the path will just be as-is.
            return File.ReadAllText(File.Exists(PathUtils.Combine(GetMappingFolder(), path)) ? PathUtils.Combine(GetMappingFolder(), path) : path);
        }

        /// <inheritdoc cref="IFileSystemHandler.FileExists"/>
        public bool FileExists(string filename)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));

            return File.Exists(AdjustPathForMappingFolder(filename));
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteFile(string, byte[])"/>
        public void WriteFile(string filename, byte[] bytes)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));
            Check.NotNull(bytes, nameof(bytes));

            File.WriteAllBytes(AdjustPathForMappingFolder(filename), bytes);
        }

        /// <inheritdoc cref="IFileSystemHandler.DeleteFile"/>
        public void DeleteFile(string filename)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));

            File.Delete(AdjustPathForMappingFolder(filename));
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadFile"/>
        public byte[] ReadFile(string filename)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));

            return File.ReadAllBytes(AdjustPathForMappingFolder(filename));
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadFileAsString"/>
        public string ReadFileAsString(string filename)
        {
            return File.ReadAllText(AdjustPathForMappingFolder(Check.NotNullOrEmpty(filename, nameof(filename))));
        }

        /// <inheritdoc cref="IFileSystemHandler.GetUnmatchedRequestsFolder"/>
        public string GetUnmatchedRequestsFolder()
        {
            return Path.Combine(_rootFolder, UnmatchedRequestsFolder);
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteUnmatchedRequest"/>
        public void WriteUnmatchedRequest(string filename, string text)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));
            Check.NotNull(text, nameof(text));

            var folder = GetUnmatchedRequestsFolder();
            Directory.CreateDirectory(folder);

            File.WriteAllText(Path.Combine(folder, filename), text);
        }

        /// <summary>
        /// Adjusts the path to the MappingFolder.
        /// </summary>
        /// <param name="filename">The path.</param>
        /// <returns>Adjusted path</returns>
        private string AdjustPathForMappingFolder(string filename)
        {
            return Path.Combine(GetMappingFolder(), filename);
        }
    }
}
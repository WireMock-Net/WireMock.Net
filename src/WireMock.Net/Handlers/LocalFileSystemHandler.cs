﻿using System.Collections.Generic;
using System.IO;
using WireMock.Validation;

namespace WireMock.Handlers
{
    /// <summary>
    /// Default implementation for a handler to interact with the local file system to read and write static mapping files.
    /// </summary>
    public class LocalFileSystemHandler : IFileSystemHandler
    {
        private static readonly string AdminMappingsFolder = Path.Combine("__admin", "mappings");

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
            path = CleanPath(path);
            // If the file exists at the given path relative to the MappingsFolder, then return that.
            // Else the path will just be as-is.
            return File.ReadAllBytes(File.Exists(Path.Combine(GetMappingFolder(), path)) ? Path.Combine(GetMappingFolder(), path) : path);
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadResponseBodyAsString"/>
        public string ReadResponseBodyAsString(string path)
        {
            Check.NotNullOrEmpty(path, nameof(path));
            path = CleanPath(path);
            // In case the path is a filename, the path will be adjusted to the MappingFolder.
            // Else the path will just be as-is.
            return File.ReadAllText(File.Exists(Path.Combine(GetMappingFolder(), path)) ? Path.Combine(GetMappingFolder(), path) : path);
        }

        /// <inheritdoc cref="IFileSystemHandler.FileExists"/>
        public bool FileExists(string filename)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));

            return File.Exists(AdjustPath(filename));
        }

        /// <inheritdoc cref="IFileSystemHandler.WriteFile(string, byte[])"/>
        public void WriteFile(string filename, byte[] bytes)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));
            Check.NotNull(bytes, nameof(bytes));

            File.WriteAllBytes(AdjustPath(filename), bytes);
        }

        /// <inheritdoc cref="IFileSystemHandler.DeleteFile"/>
        public void DeleteFile(string filename)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));

            File.Delete(AdjustPath(filename));
        }

        /// <inheritdoc cref="IFileSystemHandler.ReadFile"/>
        public byte[] ReadFile(string filename)
        {
            Check.NotNullOrEmpty(filename, nameof(filename));

            return File.ReadAllBytes(AdjustPath(filename));
        }

        /// <summary>
        /// Adjusts the path to the MappingFolder.
        /// </summary>
        /// <param name="filename">The path.</param>
        /// <returns>Adjusted path</returns>
        private string AdjustPath(string filename)
        {
            return Path.Combine(GetMappingFolder(), filename);
        }

        /// <summary>
        /// Robust handling of the user defined path.
        /// Gets the path string ready for Path.Combine method.
        /// Also supports Unix and Windows platforms
        /// </summary>
        /// <param name="path">Path to clean</param>
        /// <returns></returns>
        private string CleanPath(string path)
        {
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            // remove leading directory separator character which would break Path.Combine
            path = path.StartsWith(Path.DirectorySeparatorChar.ToString()) ? path.Substring(1, path.Length - 1) : path;

            return path;
        }
    }
}

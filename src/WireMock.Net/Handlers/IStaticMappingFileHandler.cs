using System;
using System.Collections.Generic;

namespace WireMock.Handlers
{
    /// <summary>
    /// Handler to interact with the file system to read and write static mapping files.
    /// </summary>
    public interface IStaticMappingHandler
    {
        /// <summary>
        /// Gets the folder where the static mappings are located. For local file system, this would be `{CurrentFolder}/__admin/mappings`.
        /// </summary>
        /// <returns>The foldername.</returns>
        string GetMappingFolder();

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>true if path refers to an existing directory; false if the directory does not exist or an error occurs when trying to determine if the specified directory exists.</returns>
        bool FolderExists(string path);

        /// <summary>
        ///  Creates all directories and subdirectories in the specified path unless they already exist.
        /// </summary>
        /// <param name="path">The path.</param>
        void CreateFolder(string path);

        /// <summary>
        ///  Returns an enumerable collection of file names in a specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>An enumerable collection of the full names (including paths) for the files in the directory specified by path.</returns>
        IEnumerable<string> EnumerateFiles(string path);

        /// <summary>
        /// Delegate to read the static mapping by the specified path (folder + filename with .json extension)
        /// </summary>
        Func<string, string> ReadMappingFile { get; }

        /// <summary>
        /// Delegate to write the static mapping by the specified path (folder + filename with .json extension) and text;
        /// </summary>
        Action<string, string> WriteMappingFile { get; }
    }
}
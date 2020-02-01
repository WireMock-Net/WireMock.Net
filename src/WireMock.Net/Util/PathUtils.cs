using System;
using System.Collections.Generic;
using System.IO;

namespace WireMock.Util
{
    internal static class PathUtils
    {
        /// <summary>
        /// Robust handling of the user defined path.
        /// Also supports Unix and Windows platforms
        /// </summary>
        /// <param name="path">Path to clean</param>
        /// <returns></returns>
        public static string CleanPath(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Removes leading directory separator chars from the filepath, which could break Path.Combine
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RemoveLeadingDirectorySeparators(string path)
        {
            return path.TrimStart(new[] { Path.DirectorySeparatorChar });
        }
    }
}
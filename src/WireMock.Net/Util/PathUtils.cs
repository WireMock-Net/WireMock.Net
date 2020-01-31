using System;
using System.Collections.Generic;
using System.IO;

namespace WireMock.Util
{
    internal static class PathUtils
    {
        /// <summary>
        /// Robust handling of the user defined path.
        /// Gets the path string ready for Path.Combine method.
        /// Also supports Unix and Windows platforms
        /// </summary>
        /// <param name="path">Path to clean</param>
        /// <returns></returns>
        public static string CleanPath(string path)
        {
            path = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            // remove leading directory separator character which would break Path.Combine
            path = path.StartsWith(Path.DirectorySeparatorChar.ToString()) ? path.Substring(1, path.Length - 1) : path;

            return path;
        }
    }
}
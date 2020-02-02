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

        public static string Combine(string root, string path)
        {
            return Path.Combine(root, PathUtils.RemoveLeadingDirectorySeparators(path));
        }

        public static string Combine(string root, params string[] paths)
        {
            List<string> cleanPaths = new List<string>();
            cleanPaths.Add(PathUtils.CleanPath(root));

            string curPath;
            foreach(string path in paths)
            {
                curPath = PathUtils.CleanPath(path);
                curPath = PathUtils.RemoveLeadingDirectorySeparators(curPath);
                cleanPaths.Add(curPath);
            }

            return Path.Combine(cleanPaths.ToArray());
        }
    }
}
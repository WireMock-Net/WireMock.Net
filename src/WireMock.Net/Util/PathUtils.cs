using System.IO;
using Stef.Validation;

namespace WireMock.Util;

internal static class PathUtils
{
    /// <summary>
    /// Robust handling of the user defined path.
    /// Also supports Unix and Windows platforms
    /// </summary>
    /// <param name="path">The path to clean</param>
    public static string CleanPath(string path)
    {
        Guard.NotNull(path);
        return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Removes leading directory separator chars from the filepath, which could break Path.Combine
    /// </summary>
    /// <param name="path">The path to remove the loading DirectorySeparatorChars</param>
    public static string RemoveLeadingDirectorySeparators(string path)
    {
        Guard.NotNull(path);
        return path.TrimStart(new[] { Path.DirectorySeparatorChar });
    }

    /// <summary>
    /// Combine two paths
    /// </summary>
    /// <param name="root">The root path</param>
    /// <param name="path">The path</param>
    public static string Combine(string root, string path)
    {
        Guard.NotNull(root);
        Guard.NotNull(path);
        return Path.Combine(root, RemoveLeadingDirectorySeparators(path));
    }
}
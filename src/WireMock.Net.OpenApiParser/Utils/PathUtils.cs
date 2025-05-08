// Copyright © WireMock.Net

using System.Text;

namespace WireMock.Net.OpenApiParser.Utils;

internal static class PathUtils
{
    internal static string Combine(params string[] paths)
    {
        if (paths.Length == 0)
        {
            return string.Empty;
        }

        var result = new StringBuilder(paths[0].Trim().TrimEnd('/'));

        for (var i = 1; i < paths.Length; i++)
        {
            var nextPath = paths[i].Trim().TrimStart('/').TrimEnd('/');
            if (!string.IsNullOrEmpty(nextPath))
            {
                result.Append('/').Append(nextPath);
            }
        }

        return result.ToString();
    }
}
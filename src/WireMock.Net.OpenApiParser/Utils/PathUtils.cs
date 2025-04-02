// Copyright © WireMock.Net

namespace WireMock.Net.OpenApiParser.Utils;

internal static class PathUtils
{
    internal static string Combine(params string[] paths)
    {
        if (paths.Length == 0)
        {
            return string.Empty;
        }

        var result = paths[0].Trim().TrimEnd('/');

        for (int i = 1; i < paths.Length; i++)
        {
            var nextPath = paths[i].Trim().TrimStart('/').TrimEnd('/');
            if (!string.IsNullOrEmpty(nextPath))
            {
                result += '/' + nextPath;
            }
        }

        return result;
    }
}
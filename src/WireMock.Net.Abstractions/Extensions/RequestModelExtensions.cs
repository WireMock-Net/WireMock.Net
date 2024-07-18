// Copyright © WireMock.Net

using System.Linq;
using WireMock.Admin.Mappings;

namespace WireMock.Extensions;

public static class RequestModelExtensions
{
    public static string? GetPathAsString(this RequestModel request)
    {
        var path = request.Path switch
        {
            string pathAsString => pathAsString,
            PathModel pathModel => pathModel.Matchers?.FirstOrDefault()?.Pattern as string,
            _ => null
        };

        return FixPath(path);
    }

    private static string? FixPath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return path;
        }

        return path!.StartsWith("/") ? path : $"/{path}";
    }
}
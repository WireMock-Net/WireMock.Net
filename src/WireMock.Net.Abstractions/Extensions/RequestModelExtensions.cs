using System.Linq;
using WireMock.Admin.Mappings;

namespace WireMock.Extensions;

public static class RequestModelExtensions
{
    public static string? GetPathAsString(this RequestModel request)
    {
        return request.Path switch
        {
            string pathAsString => pathAsString,
            PathModel pathModel => pathModel.Matchers?.FirstOrDefault()?.Pattern as string,
            _ => null
        };
    }
}
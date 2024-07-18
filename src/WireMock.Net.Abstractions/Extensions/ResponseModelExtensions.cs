// Copyright © WireMock.Net

using WireMock.Admin.Mappings;

namespace WireMock.Extensions;

public static class ResponseModelExtensions
{
    private const string DefaultStatusCode = "200";

    public static string GetStatusCodeAsString(this ResponseModel response)
    {
        return response.StatusCode switch
        {
            string statusCodeAsString => statusCodeAsString,

            int statusCodeAsInt => statusCodeAsInt.ToString(),

            _ => response.StatusCode?.ToString() ?? DefaultStatusCode
        };
    }
}
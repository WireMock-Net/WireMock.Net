using Microsoft.OpenApi.Models;

namespace WireMock.Net.OpenApiParser.Utils;

internal interface IExampleValueGenerator
{
    object GetExampleValue(OpenApiSchema? schema);
}
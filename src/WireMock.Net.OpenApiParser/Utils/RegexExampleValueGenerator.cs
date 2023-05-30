using Microsoft.OpenApi.Models;
using Stef.Validation;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Utils;

internal class RegexExampleValueGenerator : IExampleValueGenerator
{
    public RegexExampleValueGenerator(WireMockOpenApiParserSettings settings)
    {
        Guard.NotNull(settings);
    }

    public object GetExampleValue(OpenApiSchema? schema)
    {
        switch (schema?.GetSchemaType())
        {
            case SchemaType.Boolean:
                return @"(true|false)";

            case SchemaType.Integer:
                return @"-?\d+$";

            case SchemaType.Number:
                return @"[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?";

            default:
                return schema?.GetSchemaFormat() switch
                {
                    SchemaFormat.Date => @"(\d{4})-([01]\d)-([0-3]\d)",
                    SchemaFormat.DateTime => @"(\d{4})-([01]\d)-([0-3]\d)T([0-2]\d):([0-5]\d):([0-5]\d)(\.\d+)?(Z|[+-][0-2]\d:[0-5]\d)",
                    SchemaFormat.Byte => @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)",
                    SchemaFormat.Binary => @"[a-zA-Z0-9\+/]*={0,3}",
                    _ => ".*"
                };
        }
    }
}
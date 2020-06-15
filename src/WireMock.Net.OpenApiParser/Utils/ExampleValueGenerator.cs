using System;
using Microsoft.OpenApi.Models;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Utils
{
    internal static class ExampleValueGenerator
    {
        public static object GetExampleValue(OpenApiSchema schema)
        {
            switch (schema?.GetSchemaType())
            {
                case SchemaType.Boolean:
                    return true;

                case SchemaType.Integer:
                    return 42;

                case SchemaType.Number:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Float:
                            return 4.2f;

                        default:
                            return 4.2d;
                    }

                default:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Date:
                            return DateTimeUtils.ToRfc3339Date(DateTime.UtcNow);

                        case SchemaFormat.DateTime:
                            return DateTimeUtils.ToRfc3339DateTime(DateTime.UtcNow);

                        case SchemaFormat.Byte:
                            return new byte[] { 48, 49, 50 };

                        case SchemaFormat.Binary:
                            return "example-object";

                        default:
                            return "example-string";
                    }
            }
        }
    }
}
// Copyright © WireMock.Net

using System.Linq;
using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Extensions;

internal static class OpenApiSchemaExtensions
{
    public static bool TryGetXNullable(this IOpenApiSchema schema, out bool value)
    {
        value = false;

        if (schema.Extensions != null && schema.Extensions.TryGetValue(OpenApiConstants.NullableExtension, out var nullExtRawValue) && nullExtRawValue is OpenApiAny { Node: { } jsonNode })
        {
            value = jsonNode.GetValueKind() == JsonValueKind.True;
            return true;
        }

        return false;
    }

    public static JsonSchemaType? GetSchemaType(this IOpenApiSchema? schema, out bool isNullable)
    {
        isNullable = false;

        if (schema == null)
        {
            return null;
        }

        if (schema.Type == null)
        {
            if (schema.AllOf?.Any() == true || schema.AnyOf?.Any() == true)
            {
                return JsonSchemaType.Object;
            }
        }

        isNullable = (schema.Type | JsonSchemaType.Null) == JsonSchemaType.Null || (schema.TryGetXNullable(out var xNullable) && xNullable);

        // Removes the Null flag from the schema.Type, ensuring the returned value represents a non-nullable type.
        return schema.Type & ~JsonSchemaType.Null;
    }

    public static SchemaFormat GetSchemaFormat(this IOpenApiSchema? schema)
    {
        switch (schema?.Format)
        {
            case "float":
                return SchemaFormat.Float;

            case "double":
                return SchemaFormat.Double;

            case "int32":
                return SchemaFormat.Int32;

            case "int64":
                return SchemaFormat.Int64;

            case "date":
                return SchemaFormat.Date;

            case "date-time":
                return SchemaFormat.DateTime;

            case "password":
                return SchemaFormat.Password;

            case "byte":
                return SchemaFormat.Byte;

            case "binary":
                return SchemaFormat.Binary;

            default:
                return SchemaFormat.Undefined;
        }
    }
}
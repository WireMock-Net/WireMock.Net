// Copyright © WireMock.Net

using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Extensions;

internal static class OpenApiSchemaExtensions
{
    /// <summary>
    /// https://stackoverflow.com/questions/48111459/how-to-define-a-property-that-can-be-string-or-null-in-openapi-swagger
    /// </summary>
    public static bool TryGetXNullable(this OpenApiSchema schema, out bool value)
    {
        value = false;

        if (schema.Extensions.TryGetValue("x-nullable", out var e) && e is OpenApiBoolean openApiBoolean)
        {
            value = openApiBoolean.Value;
            return true;
        }

        return false;
    }

    public static SchemaType GetSchemaType(this OpenApiSchema? schema)
    {
        if (schema == null)
        {
            return SchemaType.Unknown;
        }

        if (schema.Type == null)
        {
            if (schema.AllOf.Any() || schema.AnyOf.Any())
            {
                return SchemaType.Object;
            }
        }

        return schema.Type switch
        {
            "object" => SchemaType.Object,
            "array" => SchemaType.Array,
            "integer" => SchemaType.Integer,
            "number" => SchemaType.Number,
            "boolean" => SchemaType.Boolean,
            "string" => SchemaType.String,
            "file" => SchemaType.File,
            _ => SchemaType.Unknown
        };
    }

    public static SchemaFormat GetSchemaFormat(this OpenApiSchema? schema)
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
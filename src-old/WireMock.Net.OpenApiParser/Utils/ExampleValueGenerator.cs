// Copyright © WireMock.Net

using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Stef.Validation;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Utils;

internal class ExampleValueGenerator
{
    private readonly IWireMockOpenApiParserExampleValues _exampleValues;

    public ExampleValueGenerator(WireMockOpenApiParserSettings settings)
    {
        Guard.NotNull(settings);

        // Check if user provided an own implementation
        if (settings.ExampleValues is null)
        {
            if (settings.DynamicExamples)
            {
                _exampleValues = new WireMockOpenApiParserDynamicExampleValues();
            }
            else
            {
                _exampleValues = new WireMockOpenApiParserExampleValues();
            }
        }
        else
        {
            _exampleValues = settings.ExampleValues;
        }
    }

    public object GetExampleValue(OpenApiSchema? schema)
    {
        var schemaExample = schema?.Example;
        var schemaEnum = schema?.Enum?.FirstOrDefault();

        _exampleValues.Schema = schema;

        switch (schema?.GetSchemaType())
        {
            case SchemaType.Boolean:
                var exampleBoolean = schemaExample as OpenApiBoolean;
                return exampleBoolean?.Value ?? _exampleValues.Boolean;

            case SchemaType.Integer:
                switch (schema?.GetSchemaFormat())
                {
                    case SchemaFormat.Int64:
                        var exampleLong = schemaExample as OpenApiLong;
                        var enumLong = schemaEnum as OpenApiLong;
                        var valueLongEnumOrExample = enumLong?.Value ?? exampleLong?.Value;
                        return valueLongEnumOrExample ?? _exampleValues.Integer;

                    default:
                        var exampleInteger = schemaExample as OpenApiInteger;
                        var enumInteger = schemaEnum as OpenApiInteger;
                        var valueIntegerEnumOrExample = enumInteger?.Value ?? exampleInteger?.Value;
                        return valueIntegerEnumOrExample ?? _exampleValues.Integer;
                }

            case SchemaType.Number:
                switch (schema?.GetSchemaFormat())
                {
                    case SchemaFormat.Float:
                        var exampleFloat = schemaExample as OpenApiFloat;
                        var enumFloat = schemaEnum as OpenApiFloat;
                        var valueFloatEnumOrExample = enumFloat?.Value ?? exampleFloat?.Value;
                        return valueFloatEnumOrExample ?? _exampleValues.Float;

                    default:
                        var exampleDouble = schemaExample as OpenApiDouble;
                        var enumDouble = schemaEnum as OpenApiDouble;
                        var valueDoubleEnumOrExample = enumDouble?.Value ?? exampleDouble?.Value;
                        return valueDoubleEnumOrExample ?? _exampleValues.Double;
                }

            default:
                switch (schema?.GetSchemaFormat())
                {
                    case SchemaFormat.Date:
                        var exampleDate = schemaExample as OpenApiDate;
                        var enumDate = schemaEnum as OpenApiDate;
                        var valueDateEnumOrExample = enumDate?.Value ?? exampleDate?.Value;
                        return DateTimeUtils.ToRfc3339Date(valueDateEnumOrExample ?? _exampleValues.Date());

                    case SchemaFormat.DateTime:
                        var exampleDateTime = schemaExample as OpenApiDateTime;
                        var enumDateTime = schemaEnum as OpenApiDateTime;
                        var valueDateTimeEnumOrExample = enumDateTime?.Value ?? exampleDateTime?.Value;
                        return DateTimeUtils.ToRfc3339DateTime(valueDateTimeEnumOrExample?.DateTime ?? _exampleValues.DateTime());

                    case SchemaFormat.Byte:
                        var exampleByte = schemaExample as OpenApiByte;
                        var enumByte = schemaEnum as OpenApiByte;
                        var valueByteEnumOrExample = enumByte?.Value ?? exampleByte?.Value;
                        return valueByteEnumOrExample ?? _exampleValues.Bytes;

                    case SchemaFormat.Binary:
                        var exampleBinary = schemaExample as OpenApiBinary;
                        var enumBinary = schemaEnum as OpenApiBinary;
                        var valueBinaryEnumOrExample = enumBinary?.Value ?? exampleBinary?.Value;
                        return valueBinaryEnumOrExample ?? _exampleValues.Object;

                    default:
                        var exampleString = schemaExample as OpenApiString;
                        var enumString = schemaEnum as OpenApiString;
                        var valueStringEnumOrExample = enumString?.Value ?? exampleString?.Value;
                        return valueStringEnumOrExample ?? _exampleValues.String;
                }
        }
    }
}
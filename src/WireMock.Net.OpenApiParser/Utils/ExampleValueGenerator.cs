using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Utils
{
    internal class ExampleValueGenerator
    {
        private readonly WireMockOpenApiParserSettings _settings;

        public ExampleValueGenerator(WireMockOpenApiParserSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (_settings.DynamicExamples)
            {
                _settings.ExampleValues = new WireMockOpenApiParserDynamicExampleValues();
            }
            else
            {
                _settings.ExampleValues = new WireMockOpenApiParserExampleValues();
            }
        }

        public object GetExampleValue(OpenApiSchema schema)
        {
            var schemaExample = schema?.Example;
            var schemaEnum = GetRandomEnumValue(schema?.Enum);

            switch (schema?.GetSchemaType())
            {
                case SchemaType.Boolean:
                    var exampleBoolean = (OpenApiBoolean)schemaExample;
                    return exampleBoolean is null ? _settings.ExampleValues.Boolean : exampleBoolean.Value;

                case SchemaType.Integer:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Int64:
                            var exampleLong = (OpenApiLong)schemaExample;
                            var enumLong = (OpenApiLong)schemaEnum;
                            var valueLongEnumOrExample = enumLong is null ? exampleLong?.Value : enumLong?.Value;
                            return valueLongEnumOrExample ?? _settings.ExampleValues.Integer;

                        default:
                            var exampleInteger = (OpenApiInteger)schemaExample;
                            var enumInteger = (OpenApiInteger)schemaEnum;
                            var valueIntegerEnumOrExample = enumInteger is null ? exampleInteger?.Value : enumInteger?.Value;
                            return valueIntegerEnumOrExample ?? _settings.ExampleValues.Integer;
                    }

                case SchemaType.Number:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Float:
                            var exampleFloat = (OpenApiFloat)schemaExample;
                            var enumFloat = (OpenApiFloat)schemaEnum;
                            var valueFloatEnumOrExample = enumFloat is null ? exampleFloat?.Value : enumFloat?.Value;
                            return valueFloatEnumOrExample ?? _settings.ExampleValues.Float;

                        default:
                            var exampleDouble = (OpenApiDouble)schemaExample;
                            var enumDouble = (OpenApiDouble)schemaEnum;
                            var valueDoubleEnumOrExample = enumDouble is null ? exampleDouble?.Value : enumDouble?.Value;
                            return valueDoubleEnumOrExample ?? _settings.ExampleValues.Double;
                    }

                default:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Date:
                            var exampleDate = (OpenApiDate)schemaExample;
                            var enumDate = (OpenApiDate)schemaEnum;
                            var valueDateEnumOrExample = enumDate is null ? exampleDate?.Value : enumDate?.Value;
                            return DateTimeUtils.ToRfc3339Date(valueDateEnumOrExample ?? _settings.ExampleValues.Date());

                        case SchemaFormat.DateTime:
                            var exampleDateTime = (OpenApiDateTime)schemaExample;
                            var enumDateTime = (OpenApiDateTime)schemaEnum;
                            var valueDateTimeEnumOrExample = enumDateTime is null ? exampleDateTime?.Value : enumDateTime?.Value;
                            return DateTimeUtils.ToRfc3339DateTime(valueDateTimeEnumOrExample?.DateTime ?? _settings.ExampleValues.DateTime());

                        case SchemaFormat.Byte:
                            var exampleByte = (OpenApiByte)schemaExample;
                            var enumByte = (OpenApiByte)schemaEnum;
                            var valueByteEnumOrExample = enumByte is null ? exampleByte?.Value : enumByte?.Value;
                            return valueByteEnumOrExample ?? _settings.ExampleValues.Bytes;

                        case SchemaFormat.Binary:
                            var exampleBinary = (OpenApiBinary)schemaExample;
                            var enumBinary = (OpenApiBinary)schemaEnum;
                            var valueBinaryEnumOrExample = enumBinary is null ? exampleBinary?.Value : enumBinary?.Value;
                            return valueBinaryEnumOrExample ?? _settings.ExampleValues.Object;

                        default:
                            var exampleString = (OpenApiString)schemaExample;
                            var enumString = (OpenApiString)schemaEnum;
                            var valueStringEnumOrExample = enumString is null ? exampleString?.Value : enumString?.Value;
                            return valueStringEnumOrExample ?? _settings.ExampleValues.String;
                    }
            }
        }
        private static IOpenApiAny GetRandomEnumValue(IList<IOpenApiAny> schemaEnum)
        {
            if (schemaEnum?.Count > 0)
            {
                int maxValue = schemaEnum.Count - 1;
                int randomEnum = new Random().Next(0, maxValue);
                return schemaEnum[randomEnum];
            }
            return null;
        }
    }
}

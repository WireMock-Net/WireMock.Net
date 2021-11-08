using System;
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
                            return exampleLong?.Value ?? _settings.ExampleValues.Integer;
                        
                        default:
                            var exampleInteger = (OpenApiInteger)schemaExample;
                            return exampleInteger?.Value ?? _settings.ExampleValues.Integer;
                    }

                case SchemaType.Number:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Float:
                            var exampleFloat = (OpenApiFloat)schemaExample;
                            return exampleFloat?.Value ?? _settings.ExampleValues.Float;

                        default:
                            var exampleDouble = (OpenApiDouble)schemaExample;
                            return exampleDouble?.Value ?? _settings.ExampleValues.Double;
                    }

                default:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Date:
                            var exampleDate = (OpenApiDate)schemaExample;
                            return DateTimeUtils.ToRfc3339Date(exampleDate?.Value ?? _settings.ExampleValues.Date());

                        case SchemaFormat.DateTime:
                            var exampleDateTime = (OpenApiDateTime)schemaExample;
                            return DateTimeUtils.ToRfc3339DateTime(exampleDateTime?.Value.DateTime ?? _settings.ExampleValues.DateTime());

                        case SchemaFormat.Byte:
                            var exampleByte = (OpenApiByte)schemaExample;
                            return exampleByte?.Value ?? _settings.ExampleValues.Bytes;

                        case SchemaFormat.Binary:
                            var exampleBinary = (OpenApiBinary)schemaExample;
                            return exampleBinary?.Value ?? _settings.ExampleValues.Object;

                        default:
                            var exampleString = (OpenApiString)schemaExample;
                            return exampleString?.Value ?? _settings.ExampleValues.String;
                    }
            }
        }
    }
}
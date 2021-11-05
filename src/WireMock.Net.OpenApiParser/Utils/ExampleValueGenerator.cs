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
            switch (schema?.GetSchemaType())
            {
                case SchemaType.Boolean:
                    OpenApiBoolean exampleBoolean = (OpenApiBoolean)schema?.Example;
                    return exampleBoolean?.Value ?? _settings.ExampleValues.Boolean;

                case SchemaType.Integer:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Int64:
                            OpenApiLong exampleLong = (OpenApiLong)schema?.Example;
                            return exampleLong?.Value ?? _settings.ExampleValues.Integer;
                        default:
                            OpenApiInteger exampleInteger = (OpenApiInteger)schema?.Example;
                            return exampleInteger?.Value ?? _settings.ExampleValues.Integer;
                    }


                case SchemaType.Number:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Float:
                            OpenApiFloat exampleFloat = (OpenApiFloat)schema?.Example;
                            return exampleFloat?.Value ?? _settings.ExampleValues.Float;

                        default:
                            OpenApiDouble exampleDouble = (OpenApiDouble)schema?.Example;
                            return exampleDouble?.Value ?? _settings.ExampleValues.Double;
                    }

                default:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Date:
                            OpenApiDate exampleDate = (OpenApiDate)schema?.Example;
                            return DateTimeUtils.ToRfc3339Date(exampleDate?.Value ?? _settings.ExampleValues.Date());

                        case SchemaFormat.DateTime:
                            OpenApiDate exampleDateTime = (OpenApiDate)schema?.Example;
                            return DateTimeUtils.ToRfc3339DateTime(exampleDateTime?.Value ?? _settings.ExampleValues.DateTime());

                        case SchemaFormat.Byte:
                            OpenApiByte exampleByte = (OpenApiByte)schema?.Example;
                            return exampleByte?.Value ?? _settings.ExampleValues.Bytes;

                        case SchemaFormat.Binary:
                            OpenApiBinary exampleBinary = (OpenApiBinary)schema?.Example;
                            return exampleBinary?.Value ?? _settings.ExampleValues.Object;

                        default:
                            OpenApiString exampleString = (OpenApiString)schema?.Example;
                            return exampleString?.Value ?? _settings.ExampleValues.String;
                    }
            }
        }
    }
}
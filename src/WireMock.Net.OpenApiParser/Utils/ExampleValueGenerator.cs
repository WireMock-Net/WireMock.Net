using System;
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
                    return _settings.ExampleValues.Boolean;

                case SchemaType.Integer:
                    return _settings.ExampleValues.Integer;

                case SchemaType.Number:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Float:
                            return _settings.ExampleValues.Float;

                        default:
                            return _settings.ExampleValues.Double;
                    }

                default:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Date:
                            return DateTimeUtils.ToRfc3339Date(_settings.ExampleValues.Date());

                        case SchemaFormat.DateTime:
                            return DateTimeUtils.ToRfc3339DateTime(_settings.ExampleValues.DateTime());

                        case SchemaFormat.Byte:
                            return _settings.ExampleValues.Bytes;

                        case SchemaFormat.Binary:
                            return _settings.ExampleValues.Object;

                        default:
                            return _settings.ExampleValues.String;
                    }
            }
        }
    }
}
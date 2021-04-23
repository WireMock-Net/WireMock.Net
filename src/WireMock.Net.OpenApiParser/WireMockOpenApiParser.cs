using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using RamlToOpenApiConverter;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Mappers;
using WireMock.Net.OpenApiParser.Settings;

namespace WireMock.Net.OpenApiParser
{
    /// <summary>
    /// Parse a OpenApi/Swagger/V2/V3 or Raml to WireMock MappingModels.
    /// </summary>
    public class WireMockOpenApiParser : IWireMockOpenApiParser
    {
        private readonly OpenApiStreamReader _reader = new OpenApiStreamReader();

        /// <inheritdoc cref="IWireMockOpenApiParser.FromFile(string, out OpenApiDiagnostic)" />
        [PublicAPI]
        public IEnumerable<MappingModel> FromFile(string path, out OpenApiDiagnostic diagnostic)
        {
            return FromFile(path, new WireMockOpenApiParserSettings(), out diagnostic);
        }

        /// <inheritdoc cref="IWireMockOpenApiParser.FromFile(string, WireMockOpenApiParserSettings, out OpenApiDiagnostic)" />
        [PublicAPI]
        public IEnumerable<MappingModel> FromFile(string path, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
        {
            OpenApiDocument document;
            if (Path.GetExtension(path).EndsWith("raml", StringComparison.OrdinalIgnoreCase))
            {
                diagnostic = new OpenApiDiagnostic();
                document = new RamlConverter().ConvertToOpenApiDocument(path);
            }
            else
            {
                var reader = new OpenApiStreamReader();
                document = reader.Read(File.OpenRead(path), out diagnostic);
            }

            return FromDocument(document, settings);
        }

        /// <inheritdoc cref="IWireMockOpenApiParser.FromStream(Stream, out OpenApiDiagnostic)" />
        [PublicAPI]
        public IEnumerable<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic)
        {
            return FromDocument(_reader.Read(stream, out diagnostic));
        }

        /// <inheritdoc cref="IWireMockOpenApiParser.FromStream(Stream, WireMockOpenApiParserSettings, out OpenApiDiagnostic)" />
        [PublicAPI]
        public IEnumerable<MappingModel> FromStream(Stream stream, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
        {
            return FromDocument(_reader.Read(stream, out diagnostic));
        }

        /// <inheritdoc cref="IWireMockOpenApiParser.FromDocument(OpenApiDocument, WireMockOpenApiParserSettings)" />
        [PublicAPI]
        public IEnumerable<MappingModel> FromDocument(OpenApiDocument openApiDocument, WireMockOpenApiParserSettings settings = null)
        {
            return new OpenApiPathsMapper(settings).ToMappingModels(openApiDocument.Paths);
        }
    }
}
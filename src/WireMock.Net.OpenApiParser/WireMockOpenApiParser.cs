using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using RamlToOpenApiConverter;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Mappers;
using WireMock.Net.OpenApiParser.Settings;

namespace WireMock.Net.OpenApiParser;

/// <summary>
/// Parse a OpenApi/Swagger/V2/V3 or Raml to WireMock.Net MappingModels.
/// </summary>
public class WireMockOpenApiParser : IWireMockOpenApiParser
{
    private readonly OpenApiStreamReader _reader = new();

    /// <inheritdoc />
    [PublicAPI]
    public IEnumerable<MappingModel> FromFile(string path, out OpenApiDiagnostic diagnostic)
    {
        return FromFile(path, new WireMockOpenApiParserSettings(), out diagnostic);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    [PublicAPI]
    public IEnumerable<MappingModel> FromDocument(OpenApiDocument openApiDocument, WireMockOpenApiParserSettings? settings = null)
    {
        return new OpenApiPathsMapper(settings ?? new WireMockOpenApiParserSettings()).ToMappingModels(openApiDocument.Paths, openApiDocument.Servers);
    }

    /// <inheritdoc  />
    [PublicAPI]
    public IEnumerable<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic)
    {
        return FromDocument(_reader.Read(stream, out diagnostic));
    }

    /// <inheritdoc />
    [PublicAPI]
    public IEnumerable<MappingModel> FromStream(Stream stream, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
    {
        return FromDocument(_reader.Read(stream, out diagnostic), settings);
    }

    /// <inheritdoc  />
    [PublicAPI]
    public IEnumerable<MappingModel> FromText(string text, out OpenApiDiagnostic diagnostic)
    {
        return FromStream(new MemoryStream(Encoding.UTF8.GetBytes(text)), out diagnostic);
    }

    /// <inheritdoc />
    [PublicAPI]
    public IEnumerable<MappingModel> FromText(string text, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
    {
        return FromStream(new MemoryStream(Encoding.UTF8.GetBytes(text)), settings, out diagnostic);
    }
}
// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.YamlReader;
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
    private static readonly OpenApiReaderSettings ReaderSettings = new();

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<MappingModel> FromFile(string path, out OpenApiDiagnostic diagnostic)
    {
        return FromFile(path, new WireMockOpenApiParserSettings(), out diagnostic);
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<MappingModel> FromFile(string path, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
    {
        OpenApiDocument document;
        if (Path.GetExtension(path).EndsWith("raml", StringComparison.OrdinalIgnoreCase))
        {
            diagnostic = new OpenApiDiagnostic();
            document = new RamlConverter().ConvertToOpenApiDocument(path);
        }
        else
        {
            document = Read(File.OpenRead(path), out diagnostic);
        }

        return FromDocument(document, settings);
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<MappingModel> FromDocument(OpenApiDocument document, WireMockOpenApiParserSettings? settings = null)
    {
        return new OpenApiPathsMapper(settings ?? new WireMockOpenApiParserSettings()).ToMappingModels(document.Paths, document.Servers ?? []);
    }

    /// <inheritdoc  />
    [PublicAPI]
    public IReadOnlyList<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic)
    {
        return FromDocument(Read(stream, out diagnostic));
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<MappingModel> FromStream(Stream stream, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
    {
        return FromDocument(Read(stream, out diagnostic), settings);
    }

    /// <inheritdoc  />
    [PublicAPI]
    public IReadOnlyList<MappingModel> FromText(string text, out OpenApiDiagnostic diagnostic)
    {
        return FromStream(new MemoryStream(Encoding.UTF8.GetBytes(text)), out diagnostic);
    }

    /// <inheritdoc />
    [PublicAPI]
    public IReadOnlyList<MappingModel> FromText(string text, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
    {
        return FromStream(new MemoryStream(Encoding.UTF8.GetBytes(text)), settings, out diagnostic);
    }

    private static OpenApiDocument Read(Stream stream, out OpenApiDiagnostic diagnostic)
    {
        var reader = new OpenApiYamlReader();

        if (stream is not MemoryStream memoryStream)
        {
            memoryStream = ReadStreamIntoMemoryStream(stream);
        }

        var result = reader.Read(memoryStream, ReaderSettings);

        diagnostic = result.Diagnostic ?? new OpenApiDiagnostic();
        return result.Document ?? throw new InvalidOperationException("The document is null.");
    }

    private static MemoryStream ReadStreamIntoMemoryStream(Stream stream)
    {
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Settings;

namespace WireMock.Net.OpenApiParser;

/// <summary>
/// Parse a OpenApi/Swagger/V2/V3 or Raml to WireMock MappingModels.
/// </summary>
public interface IWireMockOpenApiParser
{
    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a file-path.
    /// </summary>
    /// <param name="path">The path to read the OpenApi/Swagger/V2/V3 or Raml file.</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromFile(string path, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a file-path.
    /// </summary>
    /// <param name="path">The path to read the OpenApi/Swagger/V2/V3 or Raml file.</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromFile(string path, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from an <seealso cref="OpenApiDocument"/>.
    /// </summary>
    /// <param name="document">The source OpenApiDocument</param>
    /// <param name="settings">Additional settings [optional]</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromDocument(OpenApiDocument document, WireMockOpenApiParserSettings? settings = null);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromStream(Stream stream, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="string"/>.
    /// </summary>
    /// <param name="text">The source text</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromText(string text, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="string"/>.
    /// </summary>
    /// <param name="text">The source text</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromText(string text, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic);
}
using System.Collections.Generic;
using System.IO;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Abstractions.Settings;
using WireMock.Net.OpenApiParser.Abstractions.Types;

namespace WireMock.Net.OpenApiParser.Abstractions;

/// <summary>
/// Parse a OpenApi/Swagger/V2/V3 or Raml to WireMock MappingModels.
/// </summary>
public interface IWireMockOpenApiParser
{
    /// <summary>
    /// Generate <see cref="IEnumerable{MappingModel}"/> from a file-path.
    /// </summary>
    /// <param name="path">The path to read the OpenApi/Swagger/V2/V3 or Raml file.</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IEnumerable<MappingModel> FromFile(string path, WireMockOpenApiParserSettings settings, out WireMockOpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IEnumerable{MappingModel}"/> from a <seealso cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IEnumerable<MappingModel> FromStream(Stream stream, out WireMockOpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IEnumerable{MappingModel}"/> from a <seealso cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IEnumerable<MappingModel> FromStream(Stream stream, WireMockOpenApiParserSettings settings, out WireMockOpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IEnumerable{MappingModel}"/> from a <seealso cref="string"/>.
    /// </summary>
    /// <param name="text">The source text</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IEnumerable<MappingModel> FromText(string text, out WireMockOpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IEnumerable{MappingModel}"/> from a <seealso cref="string"/>.
    /// </summary>
    /// <param name="text">The source text</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IEnumerable<MappingModel> FromText(string text, WireMockOpenApiParserSettings settings, out WireMockOpenApiDiagnostic diagnostic);
}
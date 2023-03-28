using System.Collections.Generic;

namespace WireMock.Net.OpenApiParser.Abstractions.Types;

/// <summary>
/// Object containing all diagnostic information related to Open API parsing.
/// </summary>
public class WireMockOpenApiDiagnostic
{
    /// <summary>
    /// List of all errors.
    /// </summary>
    public IReadOnlyList<WireMockOpenApiError> Errors { get; set; } = new List<WireMockOpenApiError>();

    /// <summary>
    /// Open API specification version of the document parsed.
    /// </summary>
    public string SpecificationVersion { get; set; } = "undefined";
}
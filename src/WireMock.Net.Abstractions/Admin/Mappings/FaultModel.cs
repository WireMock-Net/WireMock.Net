// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// Fault Model
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class FaultModel
{
    /// <summary>
    /// Gets or sets the fault. Can be null, "", NONE, EMPTY_RESPONSE or MALFORMED_RESPONSE_CHUNK.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the fault percentage.
    /// </summary>
    public double? Percentage { get; set; }
}
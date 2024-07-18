// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// ClientIPModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class ClientIPModel
{
    /// <summary>
    /// Gets or sets the matchers.
    /// </summary>
    public MatcherModel[]? Matchers { get; set; }

    /// <summary>
    /// The Operator to use when matchers are defined. [Optional]
    /// - null      = Same as "or".
    /// - "or"      = Only one pattern should match.
    /// - "and"     = All patterns should match.
    /// - "average" = The average value from all patterns.
    /// </summary>
    public string? MatchOperator { get; set; }
}
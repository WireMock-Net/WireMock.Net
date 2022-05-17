namespace WireMock.Admin.Mappings;

/// <summary>
/// UrlModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class UrlModel
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
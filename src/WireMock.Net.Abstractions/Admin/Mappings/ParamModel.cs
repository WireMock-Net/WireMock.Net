// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// Param Model
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class ParamModel
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Defines if the key should be matched using case-ignore.
    /// </summary>
    public bool? IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets the Reject on match for the Param Name.
    /// </summary>
    public bool? RejectOnMatch { get; set; }

    /// <summary>
    /// Gets or sets the matchers.
    /// </summary>
    public MatcherModel[]? Matchers { get; set; }
}
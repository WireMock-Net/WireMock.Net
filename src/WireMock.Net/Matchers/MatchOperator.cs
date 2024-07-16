// Copyright © WireMock.Net

namespace WireMock.Matchers;

/// <summary>
/// The Operator to use when multiple patterns are defined.
/// </summary>
public enum MatchOperator
{
    /// <summary>
    /// Only one pattern needs to  match. [Default]
    /// </summary>
    Or,

    /// <summary>
    /// All patterns should match.
    /// </summary>
    And,

    /// <summary>
    /// The average value from all patterns.
    /// </summary>
    Average
}
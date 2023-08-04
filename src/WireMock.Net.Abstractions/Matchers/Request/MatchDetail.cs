using System;

namespace WireMock.Matchers.Request;

/// <summary>
/// MatchDetail
/// </summary>
public class MatchDetail
{
    /// <summary>
    /// Gets or sets the type of the matcher.
    /// </summary>
    public Type MatcherType { get; set; }

    /// <summary>
    /// Gets or sets the score between 0.0 and 1.0
    /// </summary>
    public double Score { get; set; }
}
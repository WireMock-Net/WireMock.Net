// Copyright © WireMock.Net

namespace WireMock.Matchers;

/// <summary>
/// IObjectMatcher
/// </summary>
public interface IObjectMatcher : IMatcher
{
    /// <summary>
    /// Gets the value (can be a string or an object).
    /// </summary>
    /// <returns>Value</returns>
    object Value { get; }

    /// <summary>
    /// Determines whether the specified input is match.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>MatchResult</returns>
    MatchResult IsMatch(object? input);
}
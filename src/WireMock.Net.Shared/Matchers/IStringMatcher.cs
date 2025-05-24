// Copyright © WireMock.Net

using AnyOfTypes;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// IStringMatcher
/// </summary>
/// <inheritdoc cref="IMatcher"/>
public interface IStringMatcher : IMatcher
{
    /// <summary>
    /// Determines whether the specified input is match.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>MatchResult</returns>
    MatchResult IsMatch(string? input);

    /// <summary>
    /// Gets the patterns.
    /// </summary>
    /// <returns>Patterns</returns>
    AnyOf<string, StringPattern>[] GetPatterns();

    /// <summary>
    /// The <see cref="Matchers.MatchOperator"/>.
    /// </summary>
    MatchOperator MatchOperator { get; }
}
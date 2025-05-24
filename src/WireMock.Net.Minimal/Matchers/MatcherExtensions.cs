// Copyright © WireMock.Net

namespace WireMock.Matchers;

/// <summary>
/// Provides some extension methods for matchers.
/// </summary>
public static class MatcherExtensions
{
    /// <summary>
    /// Determines if the match result is a perfect match.
    /// </summary>
    /// <param name="matcher">The string matcher.</param>
    /// <param name="input">The input string to match.</param>
    /// <returns><c>true</c>> if the match is perfect; otherwise, <c>false</c>>.</returns>
    public static bool IsPerfectMatch(this IStringMatcher matcher, string? input)
    {
        return matcher.IsMatch(input).IsPerfect();
    }
}
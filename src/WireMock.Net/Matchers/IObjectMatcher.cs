namespace WireMock.Matchers;

/// <summary>
/// IObjectMatcher
/// </summary>
public interface IObjectMatcher : IMatcher
{
    /// <summary>
    /// Determines whether the specified input is match.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>MatchResult</returns>
    MatchResult IsMatch(object? input);
}
namespace WireMock.Matchers;

/// <summary>
/// INestedMatcher
/// </summary>
public interface INestedMatcher : IMatcher
{
    /// <summary>
    /// Get the nested matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// Determines whether the specified input is match.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
    double IsMatch(string? input);
}
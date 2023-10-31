namespace WireMock.Matchers;

/// <summary>
/// IObjectMatcher
/// </summary>
public interface IObjectMatcher : IMatcher
{
    /// <summary>
    /// Gets the value as object.
    /// </summary>
    public object? ValueAsObject { get; }

    /// <summary>
    /// Gets the value as byte[].
    /// </summary>
    public byte[]? ValueAsBytes { get; }

    /// <summary>
    /// Determines whether the specified input is match.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>MatchResult</returns>
    MatchResult IsMatch(object? input);
}
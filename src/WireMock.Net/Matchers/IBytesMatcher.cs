namespace WireMock.Matchers;

/// <summary>
/// IBytesMatcher
/// </summary>
public interface IBytesMatcher : IMatcher
{
    /// <summary>
    /// Determines whether the specified input is match.
    /// </summary>
    /// <param name="input">The input byte array.</param>
    /// <returns>MatchResult</returns>
    MatchResult IsMatch(byte[]? input);
}
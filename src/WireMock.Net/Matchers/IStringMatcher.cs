namespace WireMock.Matchers
{
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
        /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
        double IsMatch(string input);

        /// <summary>
        /// Gets the patterns.
        /// </summary>
        /// <returns>Patterns</returns>
        string[] GetPatterns();
    }
}
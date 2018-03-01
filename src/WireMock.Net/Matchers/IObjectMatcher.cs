namespace WireMock.Matchers
{
    /// <summary>
    /// IObjectMatcher
    /// </summary>
    public interface IObjectMatcher : IMatcher
    {
        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
        double IsMatch(object input);
    }
}
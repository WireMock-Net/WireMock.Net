namespace WireMock.Matchers
{
    /// <summary>
    /// IMatcher
    /// </summary>
    public interface IMatcher
    {
        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if the specified input is match; otherwise, <c>false</c>.
        /// </returns>
        bool IsMatch(string input);

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>Pattern</returns>
        string GetPattern();
    }
}
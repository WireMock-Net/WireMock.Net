namespace WireMock.Matchers
{
    /// <summary>
    /// IValueMatcher
    /// </summary>
    /// <seealso cref="IObjectMatcher" />
    public interface IValueMatcher: IObjectMatcher
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>Value</returns>
        string GetValue();
    }
}
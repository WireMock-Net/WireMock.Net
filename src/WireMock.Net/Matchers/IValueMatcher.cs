namespace WireMock.Matchers
{
    /// <summary>
    /// IValueMatcher
    /// </summary>
    /// <seealso cref="IObjectMatcher" />
    public interface IValueMatcher: IObjectMatcher
    {
        /// <summary>
        /// Gets the value (can be a string or an obejct).
        /// </summary>
        /// <returns>Value</returns>
        object Value { get; }
    }
}
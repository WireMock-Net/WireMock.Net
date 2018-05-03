namespace WireMock.Matchers
{
    /// <summary>
    /// IIgnoreCaseMatcher
    /// </summary>
    /// <inheritdoc cref="IMatcher"/>
    public interface IIgnoreCaseMatcher : IMatcher
    {
        /// <summary>
        /// Ignore the case from the pattern.
        /// </summary>
        bool IgnoreCase { get; }
    }
}
namespace WireMock.Matchers
{
    /// <summary>
    /// IIgnoreCaseMatcher
    /// </summary>
    public interface IIgnoreCaseMatcher : IMatcher
    {
        /// <summary>
        /// Ignore the case.
        /// </summary>
        bool IgnoreCase { get; }
    }
}
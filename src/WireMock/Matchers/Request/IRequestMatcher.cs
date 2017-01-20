using JetBrains.Annotations;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The RequestMatcher interface.
    /// </summary>
    public interface IRequestMatcher
    {
        /// <summary>
        /// Determines whether the specified RequestMessage is match.
        /// </summary>
        /// <param name="requestMessage">The RequestMessage.</param>
        /// <returns>
        ///   <c>true</c> if the specified RequestMessage is match; otherwise, <c>false</c>.
        /// </returns>
        bool IsMatch([NotNull] RequestMessage requestMessage);
    }
}
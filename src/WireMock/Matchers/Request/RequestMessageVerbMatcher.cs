using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request verb matcher.
    /// </summary>
    internal class RequestMessageVerbMatcher : IRequestMatcher
    {
        /// <summary>
        /// The _verb.
        /// </summary>
        private readonly string _verb;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageVerbMatcher"/> class.
        /// </summary>
        /// <param name="verb">
        /// The verb.
        /// </param>
        public RequestMessageVerbMatcher([NotNull] string verb)
        {
            Check.NotNull(verb, nameof(verb));
            _verb = verb.ToLower();
        }

        /// <summary>
        /// Determines whether the specified RequestMessage is match.
        /// </summary>
        /// <param name="requestMessage">The RequestMessage.</param>
        /// <returns>
        ///   <c>true</c> if the specified RequestMessage is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(RequestMessage requestMessage)
        {
            return requestMessage.Verb == _verb;
        }
    }
}
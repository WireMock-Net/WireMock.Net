using System.Linq;
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
        /// The verbs
        /// </summary>
        public string[] Verbs { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageVerbMatcher"/> class.
        /// </summary>
        /// <param name="verbs">
        /// The verb.
        /// </param>
        public RequestMessageVerbMatcher([NotNull] params string[] verbs)
        {
            Check.NotNull(verbs, nameof(verbs));
            Verbs = verbs.Select(v => v.ToLower()).ToArray();
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
            return Verbs.Contains(requestMessage.Verb);
        }
    }
}
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock
{
    /// <summary>
    /// The request verb spec.
    /// </summary>
    internal class RequestVerbSpec : ISpecifyRequests
    {
        /// <summary>
        /// The _verb.
        /// </summary>
        private readonly string _verb;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestVerbSpec"/> class.
        /// </summary>
        /// <param name="verb">
        /// The verb.
        /// </param>
        public RequestVerbSpec([NotNull] string verb)
        {
            Check.NotNull(verb, nameof(verb));
            _verb = verb.ToLower();
        }

        /// <summary>
        /// The is satisfied by.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSatisfiedBy(RequestMessage requestMessage)
        {
            return requestMessage.Verb == _verb;
        }
    }
}
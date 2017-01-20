using System.Collections.Generic;
using System.Linq;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The composite request matcher.
    /// </summary>
    public abstract class RequestMessageCompositeMatcher : IRequestMatcher
    {
        private readonly IEnumerable<IRequestMatcher> _requestMatchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCompositeMatcher"/> class. 
        /// The constructor.
        /// </summary>
        /// <param name="requestMatchers">
        /// The <see cref="IEnumerable&lt;IRequestMatcher&gt;"/> request matchers.
        /// </param>
        public RequestMessageCompositeMatcher(IEnumerable<IRequestMatcher> requestMatchers)
        {
            _requestMatchers = requestMatchers;
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
            return _requestMatchers.All(spec => spec.IsMatch(requestMessage));
        }
    }
}
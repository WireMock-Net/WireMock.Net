using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The composite request matcher.
    /// </summary>
    public abstract class RequestMessageCompositeMatcher : IRequestMatcher
    {
        private readonly CompositeMatcherType _type;

        /// <summary>
        /// Gets the request matchers.
        /// </summary>
        /// <value>
        /// The request matchers.
        /// </value>
        private IEnumerable<IRequestMatcher> RequestMatchers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageCompositeMatcher"/> class.
        /// </summary>
        /// <param name="requestMatchers">The request matchers.</param>
        /// <param name="type">The CompositeMatcherType type (Defaults to 'And')</param>
        protected RequestMessageCompositeMatcher([NotNull] IEnumerable<IRequestMatcher> requestMatchers, CompositeMatcherType type = CompositeMatcherType.And)
        {
            Check.NotNull(requestMatchers, nameof(requestMatchers));

            _type = type;
            RequestMatchers = requestMatchers;
        }

        /// <summary>
        /// Determines whether the specified RequestMessage is match.
        /// </summary>
        /// <param name="requestMessage">The RequestMessage.</param>
        /// <param name="requestMatchResult">The RequestMatchResult.</param>
        /// <returns>
        ///   <c>true</c> if the specified RequestMessage is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            var list = new List<bool>();
            if (_type == CompositeMatcherType.And)
            {
                foreach (var requestMatcher in RequestMatchers)
                {
                    bool isMatch = requestMatcher.IsMatch(requestMessage, requestMatchResult);
                    list.Add(isMatch);
                }

                return list.All(match => match);
            }
            
            //var orRequestMatchResult = new RequestMatchResult();
            foreach (var requestMatcher in RequestMatchers)
            {
                bool isMatch = requestMatcher.IsMatch(requestMessage, requestMatchResult);
                list.Add(isMatch);
            }

            return list.Any(match => match);
        }
    }
}
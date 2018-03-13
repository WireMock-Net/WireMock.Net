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

        /// <inheritdoc cref="IRequestMatcher.GetMatchingScore"/>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            if (!RequestMatchers.Any())
            {
                return MatchScores.Mismatch;
            }

            if (_type == CompositeMatcherType.And)
            {
                return RequestMatchers.Average(requestMatcher => requestMatcher.GetMatchingScore(requestMessage, requestMatchResult));
            }

            return RequestMatchers.Max(requestMatcher => requestMatcher.GetMatchingScore(requestMessage, requestMatchResult));
        }
    }
}
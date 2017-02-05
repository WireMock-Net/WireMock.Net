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
        /// A value between 0.0 - 1.0 of the similarity.
        /// </returns>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            var list = new List<double>();
            if (_type == CompositeMatcherType.And)
            {
                foreach (var requestMatcher in RequestMatchers)
                {
                    double score = requestMatcher.GetMatchingScore(requestMessage, requestMatchResult);
                    list.Add(score);
                }

                return list.Sum() / list.Count;
            }
            
            foreach (var requestMatcher in RequestMatchers)
            {
                double score = requestMatcher.GetMatchingScore(requestMessage, requestMatchResult);
                list.Add(score);
            }

            return list.Max();
        }
    }
}
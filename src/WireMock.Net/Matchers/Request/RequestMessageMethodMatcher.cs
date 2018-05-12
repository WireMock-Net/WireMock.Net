using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The request verb matcher.
    /// </summary>
    internal class RequestMessageMethodMatcher : IRequestMatcher
    {
        private readonly MatchBehaviour _matchBehaviour;

        /// <summary>
        /// The methods
        /// </summary>
        public string[] Methods { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageMethodMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="methods">The methods.</param>
        public RequestMessageMethodMatcher(MatchBehaviour matchBehaviour, [NotNull] params string[] methods)
        {
            Check.NotNull(methods, nameof(methods));
            _matchBehaviour = matchBehaviour;

            Methods = methods.Select(v => v.ToLower()).ToArray();
        }

        /// <inheritdoc cref="IRequestMatcher.GetMatchingScore"/>
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = MatchBehaviourHelper.Convert(_matchBehaviour, IsMatch(requestMessage));
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            return MatchScores.ToScore(Methods.Contains(requestMessage.Method));
        }
    }
}
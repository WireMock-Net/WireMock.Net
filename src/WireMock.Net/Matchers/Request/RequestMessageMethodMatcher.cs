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
        /// <summary>
        /// The methods
        /// </summary>
        public string[] Methods { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageMethodMatcher"/> class.
        /// </summary>
        /// <param name="methods">
        /// The verb.
        /// </param>
        public RequestMessageMethodMatcher([NotNull] params string[] methods)
        {
            Check.NotNull(methods, nameof(methods));
            Methods = methods.Select(v => v.ToLower()).ToArray();
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
            double score = IsMatch(requestMessage);
            requestMatchResult.TotalScore += score;

            requestMatchResult.TotalNumber++;

            return score;
        }

        private double IsMatch(RequestMessage requestMessage)
        {
            return MatchScores.ToScore(Methods.Contains(requestMessage.Method));
        }
    }
}
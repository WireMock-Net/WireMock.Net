using JetBrains.Annotations;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// The scenario and state matcher.
    /// </summary>
    internal class RequestMessageScenarioAndStateMatcher : IRequestMatcher
    {
        /// <summary>
        /// Execution state condition for the current mapping.
        /// </summary>
        [CanBeNull]
        private readonly string _executionConditionState;

        /// <summary>
        /// The next state which will be signaled after the current mapping execution.
        /// In case the value is null state will not be changed.
        /// </summary>
        [CanBeNull]
        private readonly string _nextState;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageScenarioAndStateMatcher"/> class.
        /// </summary>
        /// <param name="nextState">The next state.</param>
        /// <param name="executionConditionState">Execution state condition for the current mapping.</param>
        public RequestMessageScenarioAndStateMatcher([CanBeNull] string nextState, [CanBeNull] string executionConditionState)
        {
            _nextState = nextState;
            _executionConditionState = executionConditionState;
        }

        /// <inheritdoc />
        public double GetMatchingScore(RequestMessage requestMessage, RequestMatchResult requestMatchResult)
        {
            double score = IsMatch();
            return requestMatchResult.AddScore(GetType(), score);
        }

        private double IsMatch()
        {
            return Equals(_executionConditionState, _nextState) ? MatchScores.Perfect : MatchScores.Mismatch;
        }
    }
}
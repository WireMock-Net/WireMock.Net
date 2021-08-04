using System.Linq;

namespace WireMock.Matchers
{
    /// <summary>
    /// NotNullOrEmptyMatcher
    /// </summary>
    /// <seealso cref="IObjectMatcher" />
    public class NotNullOrEmptyMatcher : IObjectMatcher
    {
        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "NotNullOrEmptyMatcher";

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullOrEmptyMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        public NotNullOrEmptyMatcher(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
        {
            MatchBehaviour = matchBehaviour;
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            bool match;

            switch (input)
            {
                case string @string:
                    match = !string.IsNullOrEmpty(@string);
                    break;

                case byte[] bytes:
                    match = bytes != null && bytes.Any();
                    break;

                default:
                    match = input != null;
                    break;
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(match));
        }
    }
}
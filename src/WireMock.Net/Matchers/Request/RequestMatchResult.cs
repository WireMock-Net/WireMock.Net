using System;
using System.Collections.Generic;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// RequestMatchResult
    /// </summary>
    public class RequestMatchResult : IComparable
    {
        /// <summary>
        /// Gets or sets the match-score.
        /// </summary>
        /// <value>
        /// The match-score.
        /// </value>
        public double TotalScore { get; private set; }

        /// <summary>
        /// Gets or sets the total number of matches.
        /// </summary>
        /// <value>
        /// The total number of matches.
        /// </value>
        public int TotalNumber { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is perfect match.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is perfect match; otherwise, <c>false</c>.
        /// </value>
        public bool IsPerfectMatch => Math.Abs(TotalScore - TotalNumber) < MatchScores.Tolerance;

        /// <summary>
        /// Gets the match percentage.
        /// </summary>
        /// <value>
        /// The match percentage.
        /// </value>
        public double AverageTotalScore => TotalNumber == 0 ? 0.0 : TotalScore / TotalNumber;

        /// <summary>
        /// Gets the match details.
        /// </summary>
        public IList<KeyValuePair<Type, double>> MatchDetails { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMatchResult"/> class.
        /// </summary>
        public RequestMatchResult() => MatchDetails = new List<KeyValuePair<Type, double>>();

        /// <summary>
        /// Adds the score.
        /// </summary>
        /// <param name="matcherType">The matcher Type.</param>
        /// <param name="score">The score.</param>
        /// <returns>The score.</returns>
        public double AddScore(Type matcherType, double score)
        {
            TotalScore += score;
            TotalNumber++;
            MatchDetails.Add(new KeyValuePair<Type, double>(matcherType, score));

            return score;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            var compareObj = (RequestMatchResult)obj;

            return compareObj.AverageTotalScore.CompareTo(AverageTotalScore);
        }
    }
}
using System;

namespace WireMock.Matchers.Request
{
    /// <summary>
    /// RequestMatchResult
    /// </summary>
    public class RequestMatchResult : IComparable
    {
        /// <summary>
        /// Gets or sets the matches score.
        /// </summary>
        /// <value>
        /// The number of matches.
        /// </value>
        public double MatchScore { get; set; }

        /// <summary>
        /// Gets or sets the total number of matches.
        /// </summary>
        /// <value>
        /// The total number of matches.
        /// </value>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is perfect match.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is perfect match; otherwise, <c>false</c>.
        /// </value>
        public bool IsPerfectMatch => Math.Abs(MatchScore - Total) < MatchScores.Tolerance;

        /// <summary>
        /// Gets the match percentage.
        /// </summary>
        /// <value>
        /// The match percentage.
        /// </value>
        public double MatchPercentage => Total == 0 ? 1.0 : MatchScore / Total;

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CompareTo(object obj)
        {
            var compareObj = (RequestMatchResult)obj;

            return compareObj.MatchPercentage.CompareTo(MatchPercentage);
        }
    }
}
namespace WireMock.Admin.Requests
{
    /// <summary>
    /// LogRequestMatchModel
    /// </summary>
    public class LogRequestMatchModel
    {
        /// <summary>
        /// Gets or sets the number of matches.
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
        public bool IsPerfectMatch => MatchScore == Total;

        /// <summary>
        /// Gets the match percentage.
        /// </summary>
        /// <value>
        /// The match percentage.
        /// </value>
        public double MatchPercentage => Total == 0 ? 100 : 100.0 * MatchScore / Total;
    }
}
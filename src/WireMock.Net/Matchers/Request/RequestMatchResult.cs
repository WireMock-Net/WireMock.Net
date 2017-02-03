namespace WireMock.Matchers.Request
{
    /// <summary>
    /// RequestMatchResult
    /// </summary>
    public class RequestMatchResult
    {
        /// <summary>
        /// Gets or sets the number of matches.
        /// </summary>
        /// <value>
        /// The number of matches.
        /// </value>
        public int Matched { get; set; }

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
        public bool IsPerfectMatch { get; set; }
    }
}
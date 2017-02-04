namespace WireMock.Matchers
{
    /// <summary>
    /// MatchScores
    /// </summary>
    public static class MatchScores
    {
        /// <summary>
        /// The tolerance
        /// </summary>
        public const double Tolerance = 0.0001;

        /// <summary>
        /// The default mismatch score
        /// </summary>
        public const double Mismatch = 0.0;

        /// <summary>
        /// The default perfect match score
        /// </summary>
        public const double Perfect = 1.0;

        /// <summary>
        /// Convert a bool to the score.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>score</returns>
        public static double ToScore(bool value)
        {
            return value ? Perfect : Mismatch;
        }
    }
}
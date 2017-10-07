using System.Collections.Generic;
using System.Linq;

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
        /// The almost perfect match score
        /// </summary>
        public const double AlmostPerfect = 0.99;

        /// <summary>
        /// Convert a bool to the score.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>score</returns>
        public static double ToScore(bool value)
        {
            return value ? Perfect : Mismatch;
        }

        /// <summary>
        /// Calculates the score from multiple funcs.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>average score</returns>
        public static double ToScore(IEnumerable<bool> values)
        {
            return values.Any() ? values.Select(ToScore).Average() : Mismatch;
        }

        /// <summary>
        /// Calculates the score from multiple funcs.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>average score</returns>
        public static double ToScore(IEnumerable<double> values)
        {
            return values.Any() ? values.Average() : Mismatch;
        }
    }
}
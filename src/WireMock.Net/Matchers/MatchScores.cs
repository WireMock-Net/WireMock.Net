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
        /// <returns>score</returns>
        public static double ToScore(IEnumerable<bool> values)
        {
            var list = values.Select(ToScore).ToList();
            return list.Sum() / list.Count;
        }

        /// <summary>
        /// Calculates the score from multiple funcs.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>score</returns>
        public static double ToScore(IEnumerable<double> values)
        {
            var list = values.ToList();
            return list.Sum() / list.Count;
        }
    }
}
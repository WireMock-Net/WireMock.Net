using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// JSONPathMatcher
    /// </summary>
    /// <seealso cref="IMatcher" />
    public class JsonPathMatcher : IMatcher
    {
        private readonly string[] _patterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public JsonPathMatcher([NotNull] params string[] patterns)
        {
            Check.NotNull(patterns, nameof(patterns));

            _patterns = patterns;
        }

        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
        public double IsMatch(string input)
        {
            if (input == null)
                return MatchScores.Mismatch;

            try
            {
                JObject o = JObject.Parse(input);

                return MatchScores.ToScore(_patterns.Select(p => o.SelectToken(p) != null));
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <summary>
        /// Gets the patterns.
        /// </summary>
        /// <returns>Pattern</returns>
        public string[] GetPatterns()
        {
            return _patterns;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name</returns>
        public string GetName()
        {
            return "JsonPathMatcher";
        }
    }
}
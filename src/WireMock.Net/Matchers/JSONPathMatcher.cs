using System;
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
        private readonly string _pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPathMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public JsonPathMatcher([NotNull] string pattern)
        {
            Check.NotNull(pattern, nameof(pattern));

            _pattern = pattern;
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
                JToken token = o.SelectToken(_pattern);
                
                return MatchScores.ToScore(token != null);
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>Pattern</returns>
        public string GetPattern()
        {
            return _pattern;
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
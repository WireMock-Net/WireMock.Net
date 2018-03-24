using System;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// JsonPathMatcher
    /// </summary>
    /// <seealso cref="IMatcher" />
    public class JsonPathMatcher : IStringMatcher, IObjectMatcher
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

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            if (input == null)
            {
                return MatchScores.Mismatch;
            }

            try
            {
                var jtoken = JToken.Parse(input);
                return IsMatch(jtoken);
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            if (input == null)
            {
                return MatchScores.Mismatch;
            }

            try
            {
                // Check if JToken or object
                JToken jtoken = input is JToken token ? token : JObject.FromObject(input);
                return IsMatch(jtoken);
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public string[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.GetName"/>
        public string GetName()
        {
            return "JsonPathMatcher";
        }

        private double IsMatch(JToken jtoken)
        {
            // Wrap in array if needed
            JToken jarray = jtoken is JArray ? jtoken : new JArray(jtoken);

            return MatchScores.ToScore(_patterns.Select(pattern => jarray.SelectToken(pattern) != null));
        }
    }
}
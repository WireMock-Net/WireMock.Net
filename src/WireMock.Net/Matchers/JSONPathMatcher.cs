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
    public class JsonPathMatcher : IStringMatcher, IObjectMatcher
    {
        // private readonly object _jsonPattern;
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

        //public JsonPathMatcher([NotNull] object jsonPattern)
        //{
        //    Check.NotNull(jsonPattern, nameof(jsonPattern));

        //    _jsonPattern = jsonPattern;
        //}

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            if (input == null)
            {
                return MatchScores.Mismatch;
            }

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

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            if (input == null)
            {
                return MatchScores.Mismatch;
            }

            try
            {
                var o = input as JObject ?? JObject.FromObject(input);

                return MatchScores.ToScore(_patterns.Select(p => o.SelectToken(p) != null));
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
    }
}
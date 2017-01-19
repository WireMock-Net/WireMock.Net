using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// JSONPathMatcher
    /// </summary>
    /// <seealso cref="WireMock.Matchers.IMatcher" />
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
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if the specified input is match; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(string input)
        {
            if (input == null)
                return false;

            try
            {
                JObject o = JObject.Parse(input);
                JToken token = o.SelectToken(_pattern);
                
                return token != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
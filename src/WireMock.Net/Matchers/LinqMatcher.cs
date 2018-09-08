using System.Linq;
using System.Linq.Dynamic.Core;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WireMock.Util;

namespace WireMock.Matchers
{
    /// <summary>
    /// System.Linq.Dynamic.Core Expression Matcher
    /// </summary>
    /// <inheritdoc cref="IStringMatcher"/>
    public class LinqMatcher : IStringMatcher
    {
        private readonly string[] _patterns;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public LinqMatcher([NotNull] string pattern) : this(new[] { pattern })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public LinqMatcher([NotNull] string[] patterns) : this(MatchBehaviour.AcceptOnMatch, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="pattern">The pattern.</param>
        public LinqMatcher(MatchBehaviour matchBehaviour, [NotNull] string pattern) : this(matchBehaviour, new[] { pattern })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        public LinqMatcher(MatchBehaviour matchBehaviour, [NotNull] string[] patterns)
        {
            MatchBehaviour = matchBehaviour;
            _patterns = patterns;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            // Convert a single input string to a Queryable string-list with 1 entry.
            IQueryable queryable = new[] { input }.AsQueryable();

            // Use the Any(...) method to check if the result matches
            double match = MatchScores.ToScore(_patterns.Select(pattern => queryable.Any(pattern)));

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            JObject value;
            switch (input)
            {
                case JObject valueAsJObject:
                    value = valueAsJObject;
                    break;

                default:
                    value = JObject.FromObject(input);
                    break;
            }

            // Convert a single object to a Queryable JObject-list with 1 entry.
            var queryable1 = new[] { value }.AsQueryable();

            // Generate the DynamicLinq select statement.
            string dynamicSelect = JsonUtils.GenerateDynamicLinqStatement(value);

            // Execute DynamicLinq Select statement.
            var queryable2 = queryable1.Select(dynamicSelect);

            // Use the Any(...) method to check if the result matches.
            double match = MatchScores.ToScore(_patterns.Select(pattern => queryable2.Any(pattern)));

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public string[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "LinqMatcher";
    }
}

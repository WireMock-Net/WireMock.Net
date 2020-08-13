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
    /// <inheritdoc cref="IObjectMatcher"/>
    /// <inheritdoc cref="IStringMatcher"/>
    public class LinqMatcher : IObjectMatcher, IStringMatcher
    {
        private readonly string[] _patterns;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }
        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

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
        public LinqMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="pattern">The pattern.</param>
        public LinqMatcher(MatchBehaviour matchBehaviour, [NotNull] string pattern) : this(matchBehaviour, false, pattern)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        public LinqMatcher(MatchBehaviour matchBehaviour, bool throwException = false, [NotNull] params string[] patterns)
        {
            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;
            _patterns = patterns;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            double match = MatchScores.Mismatch;

            // Convert a single input string to a Queryable string-list with 1 entry.
            IQueryable queryable = new[] { input }.AsQueryable();

            try
            {
                // Use the Any(...) method to check if the result matches
                match = MatchScores.ToScore(_patterns.Select(pattern => queryable.Any(pattern)));

                return MatchBehaviourHelper.Convert(MatchBehaviour, match);
            }
            catch
            {
                if (ThrowException)
                {
                    throw;
                }
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
        public double IsMatch(object input)
        {
            double match = MatchScores.Mismatch;

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

            try
            {
                // Generate the DynamicLinq select statement.
                string dynamicSelect = JsonUtils.GenerateDynamicLinqStatement(value);

                // Execute DynamicLinq Select statement.
                var queryable2 = queryable1.Select(dynamicSelect);

                // Use the Any(...) method to check if the result matches.
                match = MatchScores.ToScore(_patterns.Select(pattern => queryable2.Any(pattern)));

                return MatchBehaviourHelper.Convert(MatchBehaviour, match);
            }
            catch
            {
                if (ThrowException)
                {
                    throw;
                }
            }

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

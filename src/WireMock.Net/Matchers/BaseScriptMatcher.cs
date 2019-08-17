using System.Linq;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Matchers
{
    internal abstract class BaseScriptMatcher : IStringMatcher
    {
        protected const string Framework = "{0}; namespace {1} {{ public class CodeHelper {{ public bool IsMatch(string it) {{ {1} }} }} }}";

        private readonly string[] _usings =
        {
            "System",
            "System.Linq",
            "System.Collections.Generic"
        };

        public MatchBehaviour MatchBehaviour { get; }

        protected readonly string[] Patterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseScriptMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        protected BaseScriptMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseScriptMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        protected BaseScriptMatcher(MatchBehaviour matchBehaviour, [NotNull] params string[] patterns)
        {
            Check.NotNull(patterns, nameof(patterns));

            MatchBehaviour = matchBehaviour;
            Patterns = patterns;
        }

        public double IsMatch(string input)
        {
            double match = MatchScores.Mismatch;

            if (input != null)
            {
                match = MatchScores.ToScore(Patterns.Select(p => IsMatch(input, p)));
            }

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public string[] GetPatterns()
        {
            return Patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public abstract string Name { get; }

        protected abstract bool IsMatch(string input, string pattern);

        protected string GetSource(string pattern)
        {
            return string.Format(Framework, string.Join(";", _usings.Distinct().Select(u => "using " + u)), pattern);
        }
    }
}
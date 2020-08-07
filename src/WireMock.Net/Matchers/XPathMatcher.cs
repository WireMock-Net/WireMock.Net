using System;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using WireMock.Validation;
#if !NETSTANDARD1_3
using Wmhelp.XPath2;
#endif

namespace WireMock.Matchers
{
    /// <summary>
    /// XPath2Matcher
    /// </summary>
    /// <seealso cref="IStringMatcher" />
    public class XPathMatcher : IStringMatcher
    {
        private readonly string[] _patterns;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public XPathMatcher([NotNull] params string[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="throwException">Throw an exception in case the internal matching fails.</param>
        /// <param name="patterns">The patterns.</param>
        public XPathMatcher(MatchBehaviour matchBehaviour, bool throwException = false, [NotNull] params string[] patterns)
        {
            Check.NotNull(patterns, nameof(patterns));

            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;
            _patterns = patterns;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            double match = MatchScores.Mismatch;
            if (input != null)
            {
                try
                {
                    var nav = new XmlDocument { InnerXml = input }.CreateNavigator();
#if NETSTANDARD1_3
                    match = MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.Evaluate($"boolean({p})"))));
#else
                    match = MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.XPath2Evaluate($"boolean({p})"))));
#endif
                }
                catch (Exception)
                {
                    if (ThrowException)
                    {
                        throw;
                    }
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
        public string Name => "XPathMatcher";
    }
}
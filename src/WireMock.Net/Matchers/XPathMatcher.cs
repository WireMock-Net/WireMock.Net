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

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public XPathMatcher([NotNull] params string[] patterns)
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
                var nav = new XmlDocument { InnerXml = input }.CreateNavigator();
#if NETSTANDARD1_3
                return MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.Evaluate($"boolean({p})"))));
#else
                return MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.XPath2Evaluate($"boolean({p})"))));
#endif
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
            return "XPathMatcher";
        }
    }
}
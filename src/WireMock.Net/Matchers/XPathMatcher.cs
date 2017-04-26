using System;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using WireMock.Validation;
#if NET45
using Wmhelp.XPath2;
#endif

namespace WireMock.Matchers
{
    /// <summary>
    /// XPath2Matcher
    /// </summary>
    /// <seealso cref="WireMock.Matchers.IMatcher" />
    public class XPathMatcher : IMatcher
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
                var nav = new XmlDocument { InnerXml = input }.CreateNavigator();
#if NET45
                return MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.XPath2Evaluate($"boolean({p})"))));
#else
                return MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.Evaluate($"boolean({p})"))));
#endif
            }
            catch (Exception)
            {
                return MatchScores.Mismatch;
            }
        }

        /// <summary>
        /// Gets the patterns.
        /// </summary>
        /// <returns>Patterns</returns>
        public string[] GetPatterns()
        {
            return _patterns;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>
        /// Name
        /// </returns>
        public string GetName()
        {
            return "XPathMatcher";
        }
    }
}
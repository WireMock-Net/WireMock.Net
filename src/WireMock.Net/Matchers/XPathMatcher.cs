using System;
using System.Xml;
using JetBrains.Annotations;
using WireMock.Validation;
using Wmhelp.XPath2;

namespace WireMock.Matchers
{
    /// <summary>
    /// XPath2Matcher
    /// </summary>
    /// <seealso cref="WireMock.Matchers.IMatcher" />
    public class XPathMatcher : IMatcher
    {
        private readonly string _pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public XPathMatcher([NotNull] string pattern)
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
                var nav = new XmlDocument { InnerXml = input }.CreateNavigator();
                object result = nav.XPath2Evaluate($"boolean({_pattern})");

                return MatchScores.ToScore(true.Equals(result));
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
        /// <returns>
        /// Name
        /// </returns>
        public string GetName()
        {
            return "XPathMatcher";
        }
    }
}
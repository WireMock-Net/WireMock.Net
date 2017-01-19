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
                var nav = new XmlDocument { InnerXml = input }.CreateNavigator();
                object result = nav.XPath2Evaluate($"boolean({_pattern})");

                return true.Equals(result);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
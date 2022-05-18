using System;
using System.Linq;
using System.Xml;
using AnyOfTypes;
using WireMock.Extensions;
using WireMock.Models;
using Stef.Validation;
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
        private readonly AnyOf<string, StringPattern>[] _patterns;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public XPathMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
        /// <param name="patterns">The patterns.</param>
        public XPathMatcher(
            MatchBehaviour matchBehaviour,
            bool throwException = false,
            MatchOperator matchOperator = MatchOperator.Or,
            params AnyOf<string, StringPattern>[] patterns)
        {
            _patterns = Guard.NotNull(patterns);
            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;
            MatchOperator = matchOperator;
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
                    match = MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.Evaluate($"boolean({p.GetPattern()})"))), MatchOperator);
#else
                    match = MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.XPath2Evaluate($"boolean({p.GetPattern()})"))), MatchOperator);
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
        public AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc />
        public MatchOperator MatchOperator { get; }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => "XPathMatcher";
    }
}
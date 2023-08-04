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

        /// <inheritdoc />
        public MatchBehaviour MatchBehaviour { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        public XPathMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
        /// <param name="patterns">The patterns.</param>
        public XPathMatcher(
            MatchBehaviour matchBehaviour,
            MatchOperator matchOperator = MatchOperator.Or,
            params AnyOf<string, StringPattern>[] patterns)
        {
            _patterns = Guard.NotNull(patterns);
            MatchBehaviour = matchBehaviour;
            MatchOperator = matchOperator;
        }

        /// <inheritdoc />
        public MatchResult IsMatch(string? input)
        {
            var score = MatchScores.Mismatch;
            Exception? exception = null;

            if (input != null)
            {
                try
                {
                    var nav = new XmlDocument { InnerXml = input }.CreateNavigator();
#if NETSTANDARD1_3
                    score = MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.Evaluate($"boolean({p.GetPattern()})"))).ToArray(), MatchOperator);
#else
                    score = MatchScores.ToScore(_patterns.Select(p => true.Equals(nav.XPath2Evaluate($"boolean({p.GetPattern()})"))).ToArray(), MatchOperator);
#endif
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
        }

        /// <inheritdoc />
        public AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc />
        public MatchOperator MatchOperator { get; }

        /// <inheritdoc />
        public string Name => nameof(XPathMatcher);
    }
}
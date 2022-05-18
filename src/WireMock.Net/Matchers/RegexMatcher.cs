using System;
using System.Linq;
using System.Text.RegularExpressions;
using AnyOfTypes;
using JetBrains.Annotations;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.RegularExpressions;
using Stef.Validation;

namespace WireMock.Matchers;

/// <summary>
/// Regular Expression Matcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
/// <inheritdoc cref="IIgnoreCaseMatcher"/>
public class RegexMatcher : IStringMatcher, IIgnoreCaseMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;
    private readonly Regex[] _expressions;

    /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc cref="IMatcher.ThrowException"/>
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="useRegexExtended">Use RegexExtended (default = true).</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public RegexMatcher(
        [RegexPattern] AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        bool throwException = false,
        bool useRegexExtended = true,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(MatchBehaviour.AcceptOnMatch, new[] { pattern }, ignoreCase, throwException, useRegexExtended, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="useRegexExtended">Use RegexExtended (default = true).</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public RegexMatcher(
        MatchBehaviour matchBehaviour,
        [RegexPattern] AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        bool throwException = false,
        bool useRegexExtended = true,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(matchBehaviour, new[] { pattern }, ignoreCase, throwException, useRegexExtended, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="useRegexExtended">Use RegexExtended (default = true).</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public RegexMatcher(
        MatchBehaviour matchBehaviour,
        [RegexPattern] AnyOf<string, StringPattern>[] patterns,
        bool ignoreCase = false,
        bool throwException = false,
        bool useRegexExtended = true,
        MatchOperator matchOperator = MatchOperator.Or)
    {
        _patterns = Guard.NotNull(patterns);
        IgnoreCase = ignoreCase;
        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
        MatchOperator = matchOperator;

        RegexOptions options = RegexOptions.Compiled | RegexOptions.Multiline;

        if (ignoreCase)
        {
            options |= RegexOptions.IgnoreCase;
        }

        _expressions = patterns.Select(p => useRegexExtended ? new RegexExtended(p.GetPattern(), options) : new Regex(p.GetPattern(), options)).ToArray();
    }

    /// <inheritdoc cref="IStringMatcher.IsMatch"/>
    public virtual double IsMatch(string input)
    {
        double match = MatchScores.Mismatch;
        if (input != null)
        {
            try
            {
                match = MatchScores.ToScore(_expressions.Select(e => e.IsMatch(input)), MatchOperator);
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

    /// <inheritdoc />
    public virtual AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public virtual string Name => nameof(RegexMatcher);

    /// <inheritdoc />
    public bool IgnoreCase { get; }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

}
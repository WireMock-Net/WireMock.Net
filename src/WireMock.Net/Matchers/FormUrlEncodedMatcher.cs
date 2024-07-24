// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using AnyOfTypes;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// FormUrl Encoded fields Matcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
/// <inheritdoc cref="IIgnoreCaseMatcher"/>
public class FormUrlEncodedMatcher : IStringMatcher, IIgnoreCaseMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    private readonly IList<(string Key, string? Value)> _pairs;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormUrlEncodedMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public FormUrlEncodedMatcher(
        AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(MatchBehaviour.AcceptOnMatch, new[] { pattern }, ignoreCase, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormUrlEncodedMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public FormUrlEncodedMatcher(
        MatchBehaviour matchBehaviour,
        [RegexPattern] AnyOf<string, StringPattern> pattern,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or) :
        this(matchBehaviour, new[] { pattern }, ignoreCase, matchOperator)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormUrlEncodedMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public FormUrlEncodedMatcher(
        MatchBehaviour matchBehaviour,
        [RegexPattern] AnyOf<string, StringPattern>[] patterns,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or)
    {
        _patterns = Guard.NotNull(patterns);
        IgnoreCase = ignoreCase;
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;

        _pairs = new List<(string, string?)>();
        foreach (var pattern in _patterns)
        {
            if (!QueryStringParser.TryParse(pattern, IgnoreCase, out var nameValueCollection))
            {
                foreach (var nameValue in nameValueCollection!)
                {
                    _pairs.Add((nameValue.Key, nameValue.Value));
                }
            }
        }
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        // Input is null or empty and if no patterns defined, return Perfect match.
        if (string.IsNullOrEmpty(input) && _patterns.Length == 0)
        {
            return new MatchResult(MatchScores.Perfect);
        }

        if (!QueryStringParser.TryParse(input, IgnoreCase, out var inputNameValueCollection))
        {
            return new MatchResult(MatchScores.Mismatch);
        }

        Func<string, string?, bool> equals = IgnoreCase
            ? (value1, value2) => string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase)
            : (value1, value2) => value1 == value2;

        var matches = new List<bool>();
        foreach (var pair in _pairs)
        {
            var isEqual = inputNameValueCollection.ContainsKey(pair.Key) &&
                          equals(inputNameValueCollection[pair.Key], pair.Value);

            matches.Add(isEqual);
        }

        var score = MatchScores.ToScore(matches.ToArray(), MatchOperator);
        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score));
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
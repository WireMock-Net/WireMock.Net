// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request clientIP matcher.
/// </summary>
public class RequestMessageClientIPMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers
    /// </summary>
    public IReadOnlyList<IStringMatcher>? Matchers { get; }

    /// <summary>
    /// The clientIP functions
    /// </summary>
    public Func<string, bool>[]? Funcs { get; }

    /// <summary>
    /// The <see cref="MatchBehaviour"/>
    /// </summary>
    public MatchBehaviour Behaviour { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="clientIPs">The clientIPs.</param>
    public RequestMessageClientIPMatcher(
        MatchBehaviour matchBehaviour,
        MatchOperator matchOperator,
        params string[] clientIPs) :
        this(matchBehaviour, matchOperator, clientIPs
            .Select(clientIP => new WildcardMatcher(matchBehaviour, new AnyOf<string, StringPattern>[] { clientIP }, false, matchOperator))
            .Cast<IStringMatcher>().ToArray())
    {
        Behaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageClientIPMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        Behaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
    /// </summary>
    /// <param name="funcs">The clientIP functions.</param>
    public RequestMessageClientIPMatcher(params Func<string, bool>[] funcs)
    {
        Funcs = Guard.NotNull(funcs);
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = GetMatchResult(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult GetMatchResult(IRequestMessage requestMessage)
    {
        if (Matchers != null)
        {
            var results = Matchers.Select(m => m.IsMatch(requestMessage.ClientIP)).ToArray();
            return MatchResult.From(results, MatchOperator);
        }

        if (Funcs != null)
        {
            var results = Funcs.Select(func => func(requestMessage.ClientIP)).ToArray();
            return MatchScores.ToScore(results, MatchOperator);
        }

        return default;
    }
}
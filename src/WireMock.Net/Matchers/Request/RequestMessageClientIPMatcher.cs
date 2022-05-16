using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request ClientIP matcher.
/// </summary>
public class RequestMessageClientIPMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers.
    /// </summary>
    public IReadOnlyList<IStringMatcher>? Matchers { get; }

    /// <summary>
    /// The ClientIP functions.
    /// </summary>
    public Func<string, bool>[]? Funcs { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
    /// </summary>
    /// <param name="clientIPs">The clientIPs.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    public RequestMessageClientIPMatcher(MatchBehaviour matchBehaviour, params string[] clientIPs) : this(clientIPs.Select(ip => new WildcardMatcher(matchBehaviour, ip)).Cast<IStringMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageClientIPMatcher(params IStringMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
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
        double score = IsMatch(requestMessage);
        return requestMatchResult.AddScore(GetType(), score);
    }

    private double IsMatch(IRequestMessage requestMessage)
    {
        if (Matchers != null)
        {
            return Matchers.Max(matcher => matcher.IsMatch(requestMessage.ClientIP));
        }

        if (Funcs != null)
        {
            return MatchScores.ToScore(requestMessage.ClientIP != null && Funcs.Any(func => func(requestMessage.ClientIP)));
        }

        return MatchScores.Mismatch;
    }
}
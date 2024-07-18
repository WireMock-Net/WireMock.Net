// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Matchers.Helpers;
using WireMock.Util;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body matcher.
/// </summary>
public class RequestMessageBodyMatcher : IRequestMatcher
{
    /// <summary>
    /// The body function
    /// </summary>
    public Func<string?, bool>? Func { get; }

    /// <summary>
    /// The body data function for byte[]
    /// </summary>
    public Func<byte[]?, bool>? DataFunc { get; }

    /// <summary>
    /// The body data function for json
    /// </summary>
    public Func<object?, bool>? JsonFunc { get; }

    /// <summary>
    /// The body data function for BodyData
    /// </summary>
    public Func<IBodyData?, bool>? BodyDataFunc { get; }

    /// <summary>
    /// The body data function for FormUrlEncoded
    /// </summary>
    public Func<IDictionary<string, string>?, bool>? FormUrlEncodedFunc { get; }

    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="body">The body.</param>
    public RequestMessageBodyMatcher(MatchBehaviour matchBehaviour, string body) :
        this(new[] { new WildcardMatcher(matchBehaviour, body) }.Cast<IMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="body">The body.</param>
    public RequestMessageBodyMatcher(MatchBehaviour matchBehaviour, byte[] body) :
        this(new[] { new ExactObjectMatcher(matchBehaviour, body) }.Cast<IMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="body">The body.</param>
    public RequestMessageBodyMatcher(MatchBehaviour matchBehaviour, object body) :
        this(new[] { new ExactObjectMatcher(matchBehaviour, body) }.Cast<IMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<string?, bool> func)
    {
        Func = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<byte[]?, bool> func)
    {
        DataFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<object?, bool> func)
    {
        JsonFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<IBodyData?, bool> func)
    {
        BodyDataFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<IDictionary<string, string>?, bool> func)
    {
        FormUrlEncodedFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageBodyMatcher(params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    public RequestMessageBodyMatcher(MatchOperator matchOperator, params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = CalculateMatchScore(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult CalculateMatchScore(IRequestMessage requestMessage)
    {
        if (Matchers != null && Matchers.Any())
        {
            var results = Matchers.Select(matcher => BodyDataMatchScoreCalculator.CalculateMatchScore(requestMessage.BodyData, matcher)).ToArray();
            return MatchResult.From(results, MatchOperator);
        }

        if (Func != null)
        {
            return MatchScores.ToScore(Func(requestMessage.BodyData?.BodyAsString));
        }

        if (FormUrlEncodedFunc != null)
        {
            return MatchScores.ToScore(FormUrlEncodedFunc(requestMessage.BodyData?.BodyAsFormUrlEncoded));
        }

        if (JsonFunc != null)
        {
            return MatchScores.ToScore(JsonFunc(requestMessage.BodyData?.BodyAsJson));
        }

        if (DataFunc != null)
        {
            return MatchScores.ToScore(DataFunc(requestMessage.BodyData?.BodyAsBytes));
        }

        if (BodyDataFunc != null)
        {
            return MatchScores.ToScore(BodyDataFunc(requestMessage.BodyData));
        }

        return default;
    }
}
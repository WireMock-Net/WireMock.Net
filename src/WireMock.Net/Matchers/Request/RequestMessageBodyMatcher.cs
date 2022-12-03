using System;
using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Models;
using WireMock.Types;
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
        double score = CalculateMatchScore(requestMessage);
        return requestMatchResult.AddScore(GetType(), score);
    }

    private static double CalculateMatchScore(IRequestMessage requestMessage, IMatcher matcher)
    {
        if (matcher is NotNullOrEmptyMatcher notNullOrEmptyMatcher)
        {
            switch (requestMessage.BodyData?.DetectedBodyType)
            {
                case BodyType.Json:
                case BodyType.String:
                    return notNullOrEmptyMatcher.IsMatch(requestMessage.BodyData.BodyAsString);

                case BodyType.Bytes:
                    return notNullOrEmptyMatcher.IsMatch(requestMessage.BodyData.BodyAsBytes);

                default:
                    return MatchScores.Mismatch;
            }
        }

        if (matcher is ExactObjectMatcher exactObjectMatcher)
        {
            // If the body is a byte array, try to match.
            var detectedBodyType = requestMessage.BodyData?.DetectedBodyType;
            if (detectedBodyType is BodyType.Bytes or BodyType.String)
            {
                return exactObjectMatcher.IsMatch(requestMessage.BodyData?.BodyAsBytes);
            }
        }

        // Check if the matcher is a IObjectMatcher
        if (matcher is IObjectMatcher objectMatcher)
        {
            // If the body is a JSON object, try to match.
            if (requestMessage?.BodyData?.DetectedBodyType == BodyType.Json)
            {
                return objectMatcher.IsMatch(requestMessage.BodyData.BodyAsJson);
            }

            // If the body is a byte array, try to match.
            if (requestMessage?.BodyData?.DetectedBodyType == BodyType.Bytes)
            {
                return objectMatcher.IsMatch(requestMessage.BodyData.BodyAsBytes);
            }
        }

        // Check if the matcher is a IStringMatcher
        if (matcher is IStringMatcher stringMatcher)
        {
            // If the body is a Json or a String, use the BodyAsString to match on.
            if (requestMessage?.BodyData?.DetectedBodyType == BodyType.Json || requestMessage?.BodyData?.DetectedBodyType == BodyType.String)
            {
                return stringMatcher.IsMatch(requestMessage.BodyData.BodyAsString);
            }
        }

        return MatchScores.Mismatch;
    }

    private double CalculateMatchScore(IRequestMessage requestMessage)
    {
        if (Matchers != null)
        {
            var matchersResult = Matchers.Select(matcher => CalculateMatchScore(requestMessage, matcher)).ToArray();
            return MatchScores.ToScore(matchersResult, MatchOperator);
        }

        if (Func != null)
        {
            return MatchScores.ToScore(Func(requestMessage.BodyData?.BodyAsString));
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

        return MatchScores.Mismatch;
    }
}
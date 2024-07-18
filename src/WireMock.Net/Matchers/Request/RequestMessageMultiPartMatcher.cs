// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Util;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body MultiPart matcher.
/// </summary>
public class RequestMessageMultiPartMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// The <see cref="MatchBehaviour"/>
    /// </summary>
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageMultiPartMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageMultiPartMatcher(params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageMultiPartMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageMultiPartMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
#if !MIMEKIT
        throw new System.NotSupportedException("The MultiPartMatcher can not be used for .NETStandard1.3 or .NET Framework 4.6.1 or lower.");
#else
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (Matchers?.Any() != true)
        {
            return requestMatchResult.AddScore(GetType(), score, null);
        }

        if (!MimeKitUtils.TryGetMimeMessage(requestMessage, out var message))
        {
            return requestMatchResult.AddScore(GetType(), score, null);
        }

        try
        {
            var mimePartMatchers = Matchers.OfType<MimePartMatcher>().ToArray();

            foreach (var mimePartMatcher in Matchers.OfType<MimePartMatcher>().ToArray())
            {
                score = MatchScores.Mismatch;
                foreach (var mimeBodyPart in message.BodyParts.OfType<MimeKit.MimePart>())
                {
                    var matchResult = mimePartMatcher.IsMatch(mimeBodyPart);
                    if (matchResult.IsPerfect())
                    {
                        score = MatchScores.Perfect;
                        break;
                    }
                }
                if ((MatchOperator == MatchOperator.Or && MatchScores.IsPerfect(score))
                    || (MatchOperator == MatchOperator.And && !MatchScores.IsPerfect(score)))
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return requestMatchResult.AddScore(GetType(), score, exception);
#endif
    }
}
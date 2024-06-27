#if MIMEKIT
using System;
using System.Collections.Generic;
using System.Linq;
using MimeKit;
using Stef.Validation;
using WireMock.Util;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body MultiPart matcher.
/// </summary>
public class RequestMessageMultiPartMatcher : IRequestMessageMultiPartMatcher
{
    /// <inheritdoc />
    public IMatcher[]? Matchers { get; }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// The <see cref="MatchBehaviour"/>
    /// </summary>
    public MatchBehaviour MatchBehaviour { get; }

    private readonly MimeKitUtils _mimeKitUtils = new();

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
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (Matchers?.Any() != true)
        {
            return requestMatchResult.AddScore(GetType(), score, null);
        }

        if (!_mimeKitUtils.TryGetMimeMessage(requestMessage, out MimeMessage? message))
        {
            return requestMatchResult.AddScore(GetType(), score, null);
        }

        try
        {
            var mimePartMatchers = Matchers.OfType<MimePartMatcher>().ToArray();

            foreach (var mimePart in message.BodyParts.OfType<MimePart>())
            {
                var matchesForMimePart = new List<MatchResult> { default };
                matchesForMimePart.AddRange(mimePartMatchers.Select(matcher => matcher.IsMatch(mimePart)));

                score = matchesForMimePart.Select(m => m.Score).Max();

                if (MatchScores.IsPerfect(score))
                {
                    if (MatchOperator == MatchOperator.Or)
                    {
                        break;
                    }
                }
                else
                {
                    score = MatchScores.Mismatch;
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return requestMatchResult.AddScore(GetType(), score, exception);
    }
}
#endif
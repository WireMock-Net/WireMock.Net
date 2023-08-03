using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Http;
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
        string? error = null;

        if (Matchers?.Any() != true)
        {
            return requestMatchResult.AddScore(GetType(), score, error);
        }

        try
        {
            var message = MimeKitUtils.GetMimeMessage(requestMessage.BodyData!, requestMessage.Headers![HttpKnownHeaderNames.ContentType].ToString());

            var mimePartMatchers = Matchers.OfType<MimePartMatcher>().ToArray();

            foreach (var mimePart in message.BodyParts.OfType<MimeKit.MimePart>())
            {
                var matchesForMimePart = new List<MatchResult> { MatchScores.Mismatch };
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
            error = ex.ToString();
        }

        return requestMatchResult.AddScore(GetType(), score, error);
#endif
    }
}
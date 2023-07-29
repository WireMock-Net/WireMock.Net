using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Stef.Validation;
using WireMock.Types;
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
        var match = MatchScores.Mismatch;

        if (Matchers?.Any() != true)
        {
            return requestMatchResult.AddScore(GetType(), match);
        }

        try
        {
            MimeKit.MimeMessage message;

            // If the body is a String or MultiPart, use the BodyAsString to match on.
            if (requestMessage.BodyData?.DetectedBodyType is BodyType.String or BodyType.MultiPart)
            {
                message = MimeKit.MimeMessage.Load(StreamUtils.CreateStream(requestMessage.BodyData.BodyAsString!));
            }

            // If the body is bytes, use the BodyAsBytes to match on.
            else if (requestMessage.BodyData?.DetectedBodyType is BodyType.Bytes)
            {
                var s = Encoding.UTF8.GetString(requestMessage.BodyData.BodyAsBytes!);

                message = MimeKit.MimeMessage.Load(new MemoryStream(requestMessage.BodyData.BodyAsBytes!));
            }

            // Else throw
            else
            {
                throw new NotSupportedException();
            }

            var mimePartMatchers = Matchers.OfType<MimePartMatcher>().ToArray();

            foreach (var mimePart in message.BodyParts.OfType<MimeKit.MimePart>())
            {
                var matchesForMimePart = new List<double> { MatchScores.Mismatch };
                matchesForMimePart.AddRange(mimePartMatchers.Select(matcher => matcher.IsMatch(mimePart)));

                match = matchesForMimePart.Max();

                if (MatchScores.IsPerfect(match))
                {
                    if (MatchOperator == MatchOperator.Or)
                    {
                        break;
                    }
                }
                else
                {
                    match = MatchScores.Mismatch;
                    break;
                }
            }
        }
        catch
        {
            // Empty
        }

        return requestMatchResult.AddScore(GetType(), match);
#endif
    }
}
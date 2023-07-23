#if MIMEKIT
using System.Collections.Generic;
using System.Linq;
using MimeKit;
using Stef.Validation;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// MimePartMatcher
/// </summary>
public class MultiPartMatcher : IMatcher
{
    private readonly MimePartMatcher[] _matchers;

    /// <inheritdoc />
    public string Name => nameof(MultiPartMatcher);

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPartMatcher"/> class.
    /// </summary>
    public MultiPartMatcher(MatchBehaviour matchBehaviour, MimePartMatcher[] matchers, bool throwException = false)
    {
        _matchers = Guard.NotNull(matchers);
        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
    }

    /// <summary>
    /// Determines whether the specified body is match.
    /// </summary>
    /// <param name="input">The body.</param>
    /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
    public double IsMatch(string? input)
    {
        var match = MatchScores.Mismatch;

        try
        {
            var message = MimeMessage.Load(StreamUtils.CreateStream(input!));
            foreach (var part in message.BodyParts.OfType<MimePart>())
            {
                var matchesForPath = new List<double> { MatchScores.Mismatch };
                matchesForPath.AddRange(_matchers.Select(matcher => matcher.IsMatch(part)));

                match = matchesForPath.Max();
                if (!MatchScores.IsPerfect(match))
                {
                    break;
                }
            }
        }
        catch
        {
            if (ThrowException)
            {
                throw;
            }
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, match);
    }
}
#endif
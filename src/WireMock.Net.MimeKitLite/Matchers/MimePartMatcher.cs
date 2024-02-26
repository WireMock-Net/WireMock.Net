using System;
using MimeKit;
using WireMock.Matchers.Helpers;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// MimePartMatcher
/// </summary>
public class MimePartMatcher : IMimePartMatcher
{
    private readonly Func<MimePart, MatchResult>[] _funcs;

    /// <inheritdoc />
    public string Name => nameof(MimePartMatcher);

    /// <inheritdoc />
    public IStringMatcher? ContentTypeMatcher { get; }

    /// <inheritdoc />
    public IStringMatcher? ContentDispositionMatcher { get; }

    /// <inheritdoc />
    public IStringMatcher? ContentTransferEncodingMatcher { get; }

    /// <inheritdoc />
    public IMatcher? ContentMatcher { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MimePartMatcher"/> class.
    /// </summary>
    public MimePartMatcher(
        MatchBehaviour matchBehaviour,
        IStringMatcher? contentTypeMatcher,
        IStringMatcher? contentDispositionMatcher,
        IStringMatcher? contentTransferEncodingMatcher,
        IMatcher? contentMatcher
    )
    {
        MatchBehaviour = matchBehaviour;
        ContentTypeMatcher = contentTypeMatcher;
        ContentDispositionMatcher = contentDispositionMatcher;
        ContentTransferEncodingMatcher = contentTransferEncodingMatcher;
        ContentMatcher = contentMatcher;

        _funcs = new[]
        {
            mp => ContentTypeMatcher?.IsMatch(GetContentTypeAsString(mp.ContentType)) ?? MatchScores.Perfect,
            mp => ContentDispositionMatcher?.IsMatch(mp.ContentDisposition.ToString().Replace("Content-Disposition: ", string.Empty)) ?? MatchScores.Perfect,
            mp => ContentTransferEncodingMatcher?.IsMatch(mp.ContentTransferEncoding.ToString().ToLowerInvariant()) ?? MatchScores.Perfect,
            MatchOnContent
        };
    }

    /// <summary>
    /// Determines whether the specified MimePart is match.
    /// </summary>
    /// <param name="mimePart">The MimePart.</param>
    /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
    public MatchResult IsMatch(MimePart mimePart)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        try
        {
            if (Array.TrueForAll(_funcs, func => func(mimePart).IsPerfect()))
            {
                score = MatchScores.Perfect;
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    private MatchResult MatchOnContent(MimePart mimePart)
    {
        if (ContentMatcher == null)
        {
            return MatchScores.Perfect;
        }

        var bodyParserSettings = new BodyParserSettings
        {
            Stream = mimePart.Content.Open(),
            ContentType = GetContentTypeAsString(mimePart.ContentType),
            DeserializeJson = true,
            ContentEncoding = null, // mimePart.ContentType.CharsetEncoding.ToString(),
            DecompressGZipAndDeflate = true
        };

        var bodyData = BodyParser.ParseAsync(bodyParserSettings).ConfigureAwait(false).GetAwaiter().GetResult();
        return BodyDataMatchScoreCalculator.CalculateMatchScore(bodyData, ContentMatcher);
    }

    private static string? GetContentTypeAsString(ContentType? contentType)
    {
        return contentType?.ToString().Replace("Content-Type: ", string.Empty);
    }
}
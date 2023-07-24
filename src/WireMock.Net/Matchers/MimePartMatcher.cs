#if MIMEKIT
using System;
using System.Linq;
using MimeKit;
using WireMock.Matchers.Helpers;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// MimePartMatcher
/// </summary>
public class MimePartMatcher : IMatcher
{
    private readonly Func<MimePart, double>[] _funcs;

    /// <inheritdoc />
    public string Name => nameof(MimePartMatcher);

    /// <summary>
    /// ContentType Matcher (image/png; name=image.png.)
    /// </summary>
    public IStringMatcher? ContentTypeMatcher { get; }

    /// <summary>
    /// ContentDisposition Matcher (attachment; filename=image.png)
    /// </summary>
    public IStringMatcher? ContentDispositionMatcher { get; }

    /// <summary>
    /// ContentTransferEncoding Matcher (base64)
    /// </summary>
    public IStringMatcher? ContentTransferEncodingMatcher { get; }

    /// <summary>
    /// Content Matcher
    /// </summary>
    public IMatcher? ContentMatcher { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MimePartMatcher"/> class.
    /// </summary>
    public MimePartMatcher(
        MatchBehaviour matchBehaviour,
        IStringMatcher? contentTypeMatcher,
        IStringMatcher? contentDispositionMatcher,
        IStringMatcher? contentTransferEncodingMatcher,
        IMatcher? contentMatcher,
        bool throwException = false
    )
    {
        MatchBehaviour = matchBehaviour;
        ContentTypeMatcher = contentTypeMatcher;
        ContentDispositionMatcher = contentDispositionMatcher;
        ContentTransferEncodingMatcher = contentTransferEncodingMatcher;
        ContentMatcher = contentMatcher;
        ThrowException = throwException;

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
    public double IsMatch(MimePart mimePart)
    {
        var match = MatchScores.Mismatch;

        try
        {
            if (Array.TrueForAll(_funcs, func => MatchScores.IsPerfect(func(mimePart))))
            {
                match = MatchScores.Perfect;
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

    private double MatchOnContent(MimePart mimePart)
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
#endif
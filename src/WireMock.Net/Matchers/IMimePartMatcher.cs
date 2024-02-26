namespace WireMock.Matchers;

/// <summary>
/// MimePartMatcher
/// </summary>
/// <inheritdoc cref="IMatcher"/>
public interface IMimePartMatcher : IMatcher
{
    /// <summary>
    /// ContentType Matcher (image/png; name=image.png.)
    /// </summary>
    IStringMatcher? ContentTypeMatcher { get; }

    /// <summary>
    /// ContentDisposition Matcher (attachment; filename=image.png)
    /// </summary>
    IStringMatcher? ContentDispositionMatcher { get; }

    /// <summary>
    /// ContentTransferEncoding Matcher (base64)
    /// </summary>
    IStringMatcher? ContentTransferEncodingMatcher { get; }

    /// <summary>
    /// Content Matcher
    /// </summary>
    IMatcher? ContentMatcher { get; }
}
namespace WireMock.Admin.Mappings;

/// <summary>
/// MimePartMatcherModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class MimePartMatcherModel : MatcherModel
{
    /// <summary>
    /// ContentType Matcher (image/png; name=image.png.)
    /// </summary>
    public MatcherModel? ContentTypeMatcher { get; set; }

    /// <summary>
    /// ContentDisposition Matcher (attachment; filename=image.png)
    /// </summary>
    public MatcherModel? ContentDispositionMatcher { get; set; }

    /// <summary>
    /// ContentTransferEncoding Matcher (base64)
    /// </summary>
    public MatcherModel? ContentTransferEncodingMatcher { get; set; }

    /// <summary>
    /// Content Matcher
    /// </summary>
    public MatcherModel? ContentMatcher { get; set; }
}
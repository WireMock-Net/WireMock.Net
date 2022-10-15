using System.Text;
using WireMock.Types;

namespace WireMock.Util;

/// <summary>
/// BodyData
/// </summary>
public class BodyData : IBodyData
{
    /// <inheritdoc cref="IBodyData.Encoding" />
    public Encoding? Encoding { get; set; }

    /// <inheritdoc />
    public string? BodyAsString { get; set; }

    /// <inheritdoc cref="IBodyData.BodyAsJson" />
    public object? BodyAsJson { get; set; }

    /// <inheritdoc cref="IBodyData.BodyAsBytes" />
    public byte[]? BodyAsBytes { get; set; }

    /// <inheritdoc cref="IBodyData.BodyAsJsonIndented" />
    public bool? BodyAsJsonIndented { get; set; }

    /// <inheritdoc cref="IBodyData.BodyAsFile" />
    public string? BodyAsFile { get; set; }

    /// <inheritdoc cref="IBodyData.BodyAsFileIsCached" />
    public bool? BodyAsFileIsCached { get; set; }

    /// <inheritdoc cref="IBodyData.DetectedBodyType" />
    public BodyType? DetectedBodyType { get; set; }

    /// <inheritdoc cref="IBodyData.DetectedBodyTypeFromContentType" />
    public BodyType? DetectedBodyTypeFromContentType { get; set; }

    /// <inheritdoc cref="IRequestMessage.DetectedCompression" />
    public string? DetectedCompression { get; set; }

    /// <inheritdoc />
    public string? IsFuncUsed { get; set; }
}
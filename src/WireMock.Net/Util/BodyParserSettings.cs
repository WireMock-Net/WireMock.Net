// Copyright © WireMock.Net

using System.IO;

namespace WireMock.Util;

/// <summary>
/// BodyParserSettings
/// </summary>
public class BodyParserSettings
{
    /// <summary>
    /// The body stream to parse.
    /// </summary>
    public Stream Stream { get; set; } = null!;

    /// <summary>
    /// The (optional) content type of the body.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// The (optional) content encoding of the body.
    /// </summary>
    public string? ContentEncoding { get; set; }

    /// <summary>
    /// Automatically decompress GZip and Deflate encoded content.
    /// </summary>
    public bool DecompressGZipAndDeflate { get; set; } = true;

    /// <summary>
    /// Try to deserialize the body as JSON.
    /// </summary>
    public bool DeserializeJson { get; set; } = true;

    /// <summary>
    /// Try to deserialize the body as FormUrlEncoded.
    /// </summary>
    public bool DeserializeFormUrlEncoded { get; set; } = true;
}
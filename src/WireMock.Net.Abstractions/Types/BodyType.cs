// Copyright © WireMock.Net

namespace WireMock.Types;

/// <summary>
/// The BodyType
/// </summary>
public enum BodyType
{
    /// <summary>
    /// No body present
    /// </summary>
    None,

    /// <summary>
    /// Body is a String
    /// </summary>
    String,

    /// <summary>
    /// Body is a Json object
    /// </summary>
    Json,

    /// <summary>
    /// Body is a Byte array
    /// </summary>
    Bytes,

    /// <summary>
    /// Body is a File
    /// </summary>
    File,

    /// <summary>
    /// Body is a MultiPart
    /// </summary>
    MultiPart,

    /// <summary>
    /// Body is a String which is x-www-form-urlencoded.
    /// </summary>
    FormUrlEncoded,

    /// <summary>
    /// Body is a ProtoBuf Byte array
    /// </summary>
    ProtoBuf
}
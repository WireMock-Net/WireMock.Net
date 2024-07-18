// Copyright © WireMock.Net

using System;
using System.IO;
using System.IO.Compression;

namespace WireMock.Util;

internal static class CompressionUtils
{
    public static byte[] Compress(string contentEncoding, byte[] data)
    {
        using var compressedStream = new MemoryStream();
        using var zipStream = Create(contentEncoding, compressedStream, CompressionMode.Compress);
        zipStream.Write(data, 0, data.Length);

#if !NETSTANDARD1_3
        zipStream.Close();
#endif
        return compressedStream.ToArray();
    }

    public static byte[] Decompress(string contentEncoding, byte[] data)
    {
        using var compressedStream = new MemoryStream(data);
        using var zipStream = Create(contentEncoding, compressedStream, CompressionMode.Decompress);
        using var resultStream = new MemoryStream();
        zipStream.CopyTo(resultStream);
        return resultStream.ToArray();
    }

    private static Stream Create(string contentEncoding, Stream stream, CompressionMode mode)
    {
        return contentEncoding switch
        {
            "gzip" => new GZipStream(stream, mode),
            "deflate" => new DeflateStream(stream, mode),
            _ => throw new NotSupportedException($"ContentEncoding '{contentEncoding}' is not supported.")
        };
    }
}
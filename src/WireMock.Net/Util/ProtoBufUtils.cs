#if PROTOBUF
using System;
using System.Buffers.Binary;
using JsonConverter.Abstractions;

namespace WireMock.Util;

internal static class ProtoBufUtils
{
    private const int HeaderSize = 5; // 1 (Compression flag) + 4 (UInt32)

    internal static byte[] GetProtoBufMessageWithHeader(
        string protoDefinition,
        string messageType,
        object value,
        IJsonConverter? jsonConverter,
        JsonConverterOptions? options
    )
    {
        var request = new ProtoBufJsonConverter.Models.ConvertToProtoBufRequest(protoDefinition, messageType, value);
        if (jsonConverter != null)
        {
            request = request.WithJsonConverter(jsonConverter);
            if (options != null)
            {
                request = request.WithJsonConverterOptions(options);
            }
        }

        var protobufMessageBytes = SingletonFactory<ProtoBufJsonConverter.Converter>.GetInstance().Convert(request);

        var protobufMessageWithHeaderBytes = new byte[HeaderSize + protobufMessageBytes.Length];

        WriteHeader(protobufMessageWithHeaderBytes, protobufMessageBytes.Length);

        Buffer.BlockCopy(protobufMessageBytes, 0, protobufMessageWithHeaderBytes, HeaderSize, protobufMessageBytes.Length);

        return protobufMessageWithHeaderBytes;
    }

    private static void WriteHeader(Span<byte> headerData, int length)
    {
        // Compression flag
        headerData[0] = 0;

        // Message length
        BinaryPrimitives.WriteUInt32BigEndian(headerData.Slice(1), (uint)length);
    }
}
#endif
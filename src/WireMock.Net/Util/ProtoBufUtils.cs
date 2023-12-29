#if PROTOBUF
using System;
using JsonConverter.Abstractions;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;

namespace WireMock.Util;

internal static class ProtoBufUtils
{
    internal static byte[] GetProtoBufMessageWithHeader(
        string? protoDefinition,
        string? messageType,
        object? value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null
    )
    {
        if (string.IsNullOrWhiteSpace(protoDefinition) || string.IsNullOrWhiteSpace(messageType) || value is null)
        {
            return Array.Empty<byte>();
        }

        var request = new ConvertToProtoBufRequest(protoDefinition, messageType, value, true);

        if (jsonConverter != null)
        {
            request = request.WithJsonConverter(jsonConverter);
            if (options != null)
            {
                request = request.WithJsonConverterOptions(options);
            }
        }

        return SingletonFactory<Converter>.GetInstance().Convert(request);
    }
}
#endif
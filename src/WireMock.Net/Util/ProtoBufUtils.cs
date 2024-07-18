// Copyright © WireMock.Net

#if PROTOBUF
using System;
using System.Threading;
using System.Threading.Tasks;
using JsonConverter.Abstractions;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;

namespace WireMock.Util;

internal static class ProtoBufUtils
{
    internal static async Task<byte[]> GetProtoBufMessageWithHeaderAsync(
        string? protoDefinition,
        string? messageType,
        object? value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null,
        CancellationToken cancellationToken = default
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

        return await SingletonFactory<Converter>.GetInstance().ConvertAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
#endif
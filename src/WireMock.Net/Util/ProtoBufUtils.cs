// Copyright © WireMock.Net

#if PROTOBUF
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JsonConverter.Abstractions;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;

namespace WireMock.Util;

internal static class ProtoBufUtils
{
    internal static async Task<byte[]> GetProtoBufMessageWithHeaderAsync(
        IReadOnlyList<string>? protoDefinitions,
        string? messageType,
        object? value,
        IJsonConverter? jsonConverter = null,
        CancellationToken cancellationToken = default
    )
    {
        if (protoDefinitions == null || string.IsNullOrWhiteSpace(messageType) || value is null)
        {
            return [];
        }

        var resolver = new WireMockProtoFileResolver(protoDefinitions);
        var request = new ConvertToProtoBufRequest(protoDefinitions[0], messageType, value, true)
            .WithProtoFileResolver(resolver);

        return await SingletonFactory<Converter>
            .GetInstance()
            .ConvertAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
#endif
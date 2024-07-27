using System.Threading;
using System.Threading.Tasks;
using JsonConverter.Abstractions;

namespace WireMock.Util;

internal interface IProtoBufUtils
{
    Task<byte[]> GetProtoBufMessageWithHeaderAsync(
        string? protoDefinition,
        string? messageType,
        object? value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null,
        CancellationToken cancellationToken = default
    );
}
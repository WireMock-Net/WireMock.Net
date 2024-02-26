using System.Diagnostics.CodeAnalysis;

namespace WireMock.Util;

internal interface IMimeKitUtils
{
    bool TryGetMimeMessage(IRequestMessage requestMessage, [NotNullWhen(true)] out object? mimeMessage);
}
// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WireMock.Util;

internal interface IMimeKitUtils
{
    bool TryGetMimeMessage(IRequestMessage requestMessage, [NotNullWhen(true)] out object? mimeMessage);

    IReadOnlyList<object> GetBodyParts(object mimeMessage);
}
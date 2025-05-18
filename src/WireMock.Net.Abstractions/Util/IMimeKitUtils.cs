// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WireMock.Util;

/// <summary>
/// Defines the interface for MimeKitUtils.
/// </summary>
public interface IMimeKitUtils
{
    /// <summary>
    /// Tries to get the MimeKit.MimeMessage from the request message.
    /// </summary>
    /// <param name="requestMessage">The request message.</param>
    /// <param name="mimeMessage">The MimeKit.MimeMessage</param>
    /// <returns><c>true</c> when parsed correctly, else <c>false</c></returns>
    bool TryGetMimeMessage(IRequestMessage requestMessage, [NotNullWhen(true)] out object? mimeMessage);

    /// <summary>
    /// Gets the body parts from the MimeKit.MimeMessage.
    /// </summary>
    /// <param name="mimeMessage">The MimeKit.MimeMessage.</param>
    /// <returns>A list of MimeParts.</returns>
    IReadOnlyList<object> GetBodyParts(object mimeMessage);
}
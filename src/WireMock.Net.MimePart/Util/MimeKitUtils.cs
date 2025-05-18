// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using MimeKit;
using Stef.Validation;
using WireMock.Http;
using WireMock.Types;

namespace WireMock.Util;

internal class MimeKitUtils : IMimeKitUtils
{
    /// <inheritdoc />
    public bool TryGetMimeMessage(IRequestMessage requestMessage, [NotNullWhen(true)] out object? mimeMessage)
    {
        Guard.NotNull(requestMessage);

        if (requestMessage.BodyData != null &&
            requestMessage.Headers?.TryGetValue(HttpKnownHeaderNames.ContentType, out var contentTypeHeader) == true &&
            StartsWithMultiPart(contentTypeHeader) // Only parse when "multipart/mixed"
        )
        {
            var bytes = requestMessage.BodyData?.DetectedBodyType switch
            {
                // If the body is bytes, use the BodyAsBytes to match on.
                BodyType.Bytes => requestMessage.BodyData.BodyAsBytes!,

                // If the body is a String or MultiPart, use the BodyAsString to match on.
                BodyType.String or BodyType.MultiPart => Encoding.UTF8.GetBytes(requestMessage.BodyData.BodyAsString!),

                _ => throw new NotSupportedException()
            };

            var fixedBytes = FixBytes(bytes, contentTypeHeader[0]);

            mimeMessage = MimeMessage.Load(new MemoryStream(fixedBytes));
            return true;
        }

        mimeMessage = null;
        return false;
    }

    /// <inheritdoc />
    public IReadOnlyList<object> GetBodyParts(object mimeMessage)
    {
        if (mimeMessage is not MimeMessage mm)
        {
            throw new ArgumentException($"The mimeMessage must be of type {nameof(MimeMessage)}", nameof(mimeMessage));
        }

        return mm.BodyParts
            .OfType<MimePart>()
            .ToArray();
    }

    private static bool StartsWithMultiPart(WireMockList<string> contentTypeHeader)
    {
        return contentTypeHeader.Any(ct => ct.TrimStart().StartsWith("multipart/", StringComparison.OrdinalIgnoreCase));
    }

    private static byte[] FixBytes(byte[] bytes, WireMockList<string> contentType)
    {
        var contentTypeBytes = Encoding.UTF8.GetBytes($"{HttpKnownHeaderNames.ContentType}: {contentType}\r\n\r\n");

        var result = new byte[contentTypeBytes.Length + bytes.Length];

        Buffer.BlockCopy(contentTypeBytes, 0, result, 0, contentTypeBytes.Length);
        Buffer.BlockCopy(bytes, 0, result, contentTypeBytes.Length, bytes.Length);

        return result;
    }
}
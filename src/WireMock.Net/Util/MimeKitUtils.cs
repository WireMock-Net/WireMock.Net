#if MIMEKIT
using System;
using System.IO;
using System.Text;
using MimeKit;
using WireMock.Http;
using WireMock.Types;

namespace WireMock.Util;

internal static class MimeKitUtils
{
    public static MimeMessage GetMimeMessage(IBodyData? bodyData, string contentTypeHeaderValue)
    {
        var bytes = bodyData?.DetectedBodyType switch
        {
            // If the body is bytes, use the BodyAsBytes to match on.
            BodyType.Bytes => bodyData.BodyAsBytes!,

            // If the body is a String or MultiPart, use the BodyAsString to match on.
            BodyType.String or BodyType.MultiPart => Encoding.UTF8.GetBytes(bodyData.BodyAsString!),

            _ => throw new NotSupportedException()
        };

        var fixedBytes = FixBytes(bytes, contentTypeHeaderValue);
        return MimeMessage.Load(new MemoryStream(fixedBytes));
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
#endif
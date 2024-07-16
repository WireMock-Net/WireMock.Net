// Copyright © WireMock.Net

using System.Net.Http;
using System.Net.Http.Headers;
using Stef.Validation;

namespace WireMock.Http;

internal static class ByteArrayContentHelper
{
    /// <summary>
    /// Creates a ByteArrayContent object.
    /// </summary>
    /// <param name="content">The byte[] content (cannot be null)</param>
    /// <param name="contentType">The ContentType (can be null)</param>
    /// <returns>ByteArrayContent</returns>
    internal static ByteArrayContent Create(byte[] content, MediaTypeHeaderValue? contentType)
    {
        Guard.NotNull(content);

        var byteContent = new ByteArrayContent(content);
        if (contentType != null)
        {
            byteContent.Headers.Remove(HttpKnownHeaderNames.ContentType);
            byteContent.Headers.ContentType = contentType;
        }

        return byteContent;
    }
}
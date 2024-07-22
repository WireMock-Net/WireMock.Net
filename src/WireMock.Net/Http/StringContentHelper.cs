// Copyright © WireMock.Net

using System.Net.Http;
using System.Net.Http.Headers;
using Stef.Validation;

namespace WireMock.Http;

internal static class StringContentHelper
{
    /// <summary>
    /// Creates a StringContent object.
    /// </summary>
    /// <param name="content">The string content (cannot be null)</param>
    /// <param name="contentType">The ContentType (can be null)</param>
    /// <returns>StringContent</returns>
    internal static StringContent Create(string content, MediaTypeHeaderValue? contentType)
    {
        Guard.NotNull(content);

        var stringContent = new StringContent(content);
        stringContent.Headers.ContentType = contentType;
        return stringContent;
    }
}
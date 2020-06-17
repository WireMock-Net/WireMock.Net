﻿using System.Net.Http;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.Http
{
    internal static class ByteArrayContentHelper
    {
        /// <summary>
        /// Creates a ByteArrayContent object.
        /// </summary>
        /// <param name="content">The byte[] content (cannot be null)</param>
        /// <param name="contentType">The ContentType (can be null)</param>
        /// <returns>ByteArrayContent</returns>
        internal static ByteArrayContent Create([NotNull] byte[] content, [CanBeNull] MediaTypeHeaderValue contentType)
        {
            Check.NotNull(content, nameof(content));

            var byteContent = new ByteArrayContent(content);
            if (contentType != null)
            {
                byteContent.Headers.Remove(HttpKnownHeaderNames.ContentType);
                byteContent.Headers.ContentType = contentType;
            }

            return byteContent;
        }
    }
}

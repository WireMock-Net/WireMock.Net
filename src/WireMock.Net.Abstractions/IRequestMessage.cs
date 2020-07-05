﻿using System;
using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock
{
    /// <summary>
    /// IRequestMessage
    /// </summary>
    public interface IRequestMessage
    {
        /// <summary>
        /// Gets the Client IP Address.
        /// </summary>
        string ClientIP { get; }

        /// <summary>
        /// Gets the url (relative).
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Gets the AbsoluteUrl.
        /// </summary>
        string AbsoluteUrl { get; }

        /// <summary>
        /// The ProxyUrl (if a proxy is used).
        /// </summary>
        string ProxyUrl { get; }

        /// <summary>
        /// Gets the DateTime.
        /// </summary>
        DateTime DateTime { get; }

        /// <summary>
        /// Gets the path (relative).
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the AbsolutePath.
        /// </summary>
        string AbsolutePath { get; }

        /// <summary>
        /// Gets the path segments.
        /// </summary>
        string[] PathSegments { get; }

        /// <summary>
        /// Gets the absolute path segments.
        /// </summary>
        string[] AbsolutePathSegments { get; }

        /// <summary>
        /// Gets the method.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        IDictionary<string, WireMockList<string>> Headers { get; }

        /// <summary>
        /// Gets the cookies.
        /// </summary>
        IDictionary<string, string> Cookies { get; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        IDictionary<string, WireMockList<string>> Query { get; }

        /// <summary>
        /// Gets the raw query.
        /// </summary>
        string RawQuery { get; }

        /// <summary>
        /// The body.
        /// </summary>
        IBodyData BodyData { get; }

        /// <summary>
        /// The original body as string. Convenience getter for Handlebars.
        /// </summary>
        string Body { get; }

        /// <summary>
        /// The body (as JSON object). Convenience getter for Handlebars.
        /// </summary>
        object BodyAsJson { get; }

        /// <summary>
        /// The body (as bytearray). Convenience getter for Handlebars.
        /// </summary>
        byte[] BodyAsBytes { get; }

        /// <summary>
        /// The detected body type. Convenience getter for Handlebars.
        /// </summary>
        string DetectedBodyType { get; }

        /// <summary>
        /// The detected body type from the Content-Type header. Convenience getter for Handlebars.
        /// </summary>
        string DetectedBodyTypeFromContentType { get; }

        /// <summary>
        /// The detected compression from the Content-Encoding header. Convenience getter for Handlebars.
        /// </summary>
        string DetectedCompression { get; }

        /// <summary>
        /// Gets the Host
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Gets the protocol
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// Gets the port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets the origin
        /// </summary>
        string Origin { get; }

        // WireMockList<string> GetParameter(string key, bool ignoreCase = false);
    }
}
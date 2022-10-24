// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Stef.Validation;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Types;
using WireMock.Util;

namespace WireMock;

/// <summary>
/// The RequestMessage.
/// </summary>
public class RequestMessage : IRequestMessage
{
    /// <inheritdoc cref="IRequestMessage.ClientIP" />
    public string ClientIP { get; }

    /// <inheritdoc cref="IRequestMessage.Url" />
    public string Url { get; }

    /// <inheritdoc cref="IRequestMessage.AbsoluteUrl" />
    public string AbsoluteUrl { get; }

    /// <inheritdoc cref="IRequestMessage.ProxyUrl" />
    public string ProxyUrl { get; set; }

    /// <inheritdoc cref="IRequestMessage.DateTime" />
    public DateTime DateTime { get; set; }

    /// <inheritdoc cref="IRequestMessage.Path" />
    public string Path { get; }

    /// <inheritdoc cref="IRequestMessage.AbsolutePath" />
    public string AbsolutePath { get; }

    /// <inheritdoc cref="IRequestMessage.PathSegments" />
    public string[] PathSegments { get; }

    /// <inheritdoc cref="IRequestMessage.AbsolutePathSegments" />
    public string[] AbsolutePathSegments { get; }

    /// <inheritdoc cref="IRequestMessage.Method" />
    public string Method { get; }

    /// <inheritdoc cref="IRequestMessage.Headers" />
    public IDictionary<string, WireMockList<string>>? Headers { get; }

    /// <inheritdoc cref="IRequestMessage.Cookies" />
    public IDictionary<string, string>? Cookies { get; }

    /// <inheritdoc cref="IRequestMessage.Query" />
    public IDictionary<string, WireMockList<string>>? Query { get; }

    /// <inheritdoc cref="IRequestMessage.RawQuery" />
    public string RawQuery { get; }

    /// <inheritdoc cref="IRequestMessage.BodyData" />
    public IBodyData? BodyData { get; }

    /// <inheritdoc cref="IRequestMessage.Body" />
    public string Body { get; }

    /// <inheritdoc cref="IRequestMessage.BodyAsJson" />
    public object BodyAsJson { get; }

    /// <inheritdoc cref="IRequestMessage.BodyAsBytes" />
    public byte[] BodyAsBytes { get; }

    /// <inheritdoc cref="IRequestMessage.DetectedBodyType" />
    public string DetectedBodyType { get; }

    /// <inheritdoc cref="IRequestMessage.DetectedBodyTypeFromContentType" />
    public string DetectedBodyTypeFromContentType { get; }

    /// <inheritdoc cref="IRequestMessage.DetectedCompression" />
    public string DetectedCompression { get; }

    /// <inheritdoc cref="IRequestMessage.Host" />
    public string Host { get; }

    /// <inheritdoc cref="IRequestMessage.Protocol" />
    public string Protocol { get; }

    /// <inheritdoc cref="IRequestMessage.Port" />
    public int Port { get; }

    /// <inheritdoc cref="IRequestMessage.Origin" />
    public string Origin { get; }

    /// <summary>
    /// Used for Unit Testing
    /// </summary>
    public RequestMessage(
        UrlDetails urlDetails,
        string method,
        string clientIP,
        IBodyData? bodyData = null,
        IDictionary<string, string[]>? headers = null,
        IDictionary<string, string>? cookies = null) : this(null, urlDetails, method, clientIP, bodyData, headers, cookies)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessage"/> class.
    /// </summary>
    /// <param name="options">The<seealso cref="IWireMockMiddlewareOptions"/>.</param>
    /// <param name="urlDetails">The original url details.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="clientIP">The client IP Address.</param>
    /// <param name="bodyData">The BodyData.</param>
    /// <param name="headers">The headers.</param>
    /// <param name="cookies">The cookies.</param>
    internal RequestMessage(
        IWireMockMiddlewareOptions? options,
        UrlDetails urlDetails, string method,
        string clientIP,
        IBodyData? bodyData = null,
        IDictionary<string, string[]>? headers = null,
        IDictionary<string, string>? cookies = null)
    {
        Guard.NotNull(urlDetails, nameof(urlDetails));
        Guard.NotNull(method, nameof(method));
        Guard.NotNull(clientIP, nameof(clientIP));

        AbsoluteUrl = urlDetails.AbsoluteUrl.ToString();
        Url = urlDetails.Url.ToString();
        Protocol = urlDetails.Url.Scheme;
        Host = urlDetails.Url.Host;
        Port = urlDetails.Url.Port;
        Origin = $"{Protocol}://{Host}:{Port}";

        AbsolutePath = WebUtility.UrlDecode(urlDetails.AbsoluteUrl.AbsolutePath);
        Path = WebUtility.UrlDecode(urlDetails.Url.AbsolutePath);
        PathSegments = Path.Split('/').Skip(1).ToArray();
        AbsolutePathSegments = AbsolutePath.Split('/').Skip(1).ToArray();

        Method = method;
        ClientIP = clientIP;

        BodyData = bodyData;

        // Convenience getters for e.g. Handlebars
        Body = BodyData?.BodyAsString;
        BodyAsJson = BodyData?.BodyAsJson;
        BodyAsBytes = BodyData?.BodyAsBytes;
        DetectedBodyType = BodyData?.DetectedBodyType.ToString();
        DetectedBodyTypeFromContentType = BodyData?.DetectedBodyTypeFromContentType.ToString();
        DetectedCompression = BodyData?.DetectedCompression;

        Headers = headers?.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
        Cookies = cookies;
        RawQuery = urlDetails.Url.Query;
        Query = QueryStringParser.Parse(RawQuery, options?.QueryParameterMultipleValueSupport);
    }

    /// <summary>
    /// Get a query parameter.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <returns>The query parameter.</returns>
    public WireMockList<string>? GetParameter(string key, bool ignoreCase = false)
    {
        if (Query == null)
        {
            return null;
        }

        var query = !ignoreCase ? Query : new Dictionary<string, WireMockList<string>>(Query, StringComparer.OrdinalIgnoreCase);

        return query.ContainsKey(key) ? query[key] : null;
    }
}
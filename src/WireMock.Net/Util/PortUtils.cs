// Copyright © WireMock.Net

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using WireMock.Constants;

namespace WireMock.Util;

/// <summary>
/// Port Utility class
/// </summary>
internal static class PortUtils
{
    private static readonly Regex UrlDetailsRegex = new(@"^((?<proto>\w+)://)(?<host>[^/]+?):(?<port>\d+)\/?$", RegexOptions.Compiled, WireMockConstants.DefaultRegexTimeout);

    /// <summary>
    /// Finds a free TCP port.
    /// </summary>
    /// <remarks>see http://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net.</remarks>
    public static int FindFreeTcpPort()
    {
        TcpListener? tcpListener = null;
        try
        {
            tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();

            return ((IPEndPoint)tcpListener.LocalEndpoint).Port;
        }
        finally
        {
            tcpListener?.Stop();
        }
    }

    /// <summary>
    /// Extract the isHttps, isHttp2, protocol, host and port from a URL.
    /// </summary>
    public static bool TryExtract(string url, out bool isHttps, out bool isHttp2, [NotNullWhen(true)] out string? protocol, [NotNullWhen(true)] out string? host, out int port)
    {
        isHttps = false;
        isHttp2 = false;
        protocol = null;
        host = null;
        port = default;

        var match = UrlDetailsRegex.Match(url);
        if (match.Success)
        {
            protocol = match.Groups["proto"].Value;
            isHttps = protocol.StartsWith("https", StringComparison.OrdinalIgnoreCase) || protocol.StartsWith("grpcs", StringComparison.OrdinalIgnoreCase);
            isHttp2 = protocol.StartsWith("grpc", StringComparison.OrdinalIgnoreCase);
            host = match.Groups["host"].Value;

            return int.TryParse(match.Groups["port"].Value, out port);
        }

        return false;
    }
}
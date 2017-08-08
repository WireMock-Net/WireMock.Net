﻿using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace WireMock.Http
{
    /// <summary>
    /// Utility class
    /// </summary>
    public static class PortUtil
    {
        private static readonly Regex UrlDetailsRegex = new Regex(@"^(?<proto>\w+)://[^/]+?(?<port>\d+)?/", RegexOptions.Compiled);

        /// <summary>
        /// The find free TCP port.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <remarks>see http://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net.</remarks>
        public static int FindFreeTcpPort()
        {
            TcpListener tcpListener = null;
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
        /// Extract a proto and port from a URL.
        /// </summary>
        public static bool TryExtractProtocolAndPort(string url, out string proto, out int port)
        {
            proto = null;
            port = -1;

            Match m = UrlDetailsRegex.Match(url);
            if (m.Success)
            {
                proto = m.Groups["proto"].Value;

                return int.TryParse(m.Groups["port"].Value, out port);
            }

            return false;
        }
    }
}
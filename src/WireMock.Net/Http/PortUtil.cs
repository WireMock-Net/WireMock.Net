using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace WireMock.Http
{
    /// <summary>
    /// Port Utility class
    /// </summary>
    public static class PortUtil
    {
        private static readonly IPEndPoint DefaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, port: 0);
        private static readonly Regex UrlDetailsRegex = new Regex(@"^(?<proto>\w+)://[^/]+?(?<port>\d+)?/", RegexOptions.Compiled);

        /// <summary>
        /// Finds a free TCP port.
        /// </summary>
        /// <remarks>see http://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net.</remarks>
        public static int FindFreeTcpPort()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(DefaultLoopbackEndpoint);
                return ((IPEndPoint)socket.LocalEndPoint).Port;
            }
        }

        /// <summary>
        /// Extract a proto and port from a URL.
        /// </summary>
        public static bool TryExtractProtocolAndPort(string url, out string proto, out int port)
        {
            proto = null;
            port = 0;

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
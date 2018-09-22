using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace WireMock.Util
{
    /// <summary>
    /// Port Utility class
    /// </summary>
    public static class PortUtils
    {
        private static readonly Regex UrlDetailsRegex = new Regex(@"^((?<proto>\w+)://)(?<host>[^/]+?):(?<port>\d+)\/?$", RegexOptions.Compiled);

        /// <summary>
        /// Finds a free TCP port.
        /// </summary>
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
        /// Extract the protocol, host and port from a URL.
        /// </summary>
        public static bool TryExtract(string url, out string protocol, out string host, out int port)
        {
            protocol = null;
            host = null;
            port = default(int);

            Match m = UrlDetailsRegex.Match(url);
            if (m.Success)
            {
                protocol = m.Groups["proto"].Value;
                host = m.Groups["host"].Value;

                return int.TryParse(m.Groups["port"].Value, out port);
            }

            return false;
        }
    }
}
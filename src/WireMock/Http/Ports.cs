using System.Net;
using System.Net.Sockets;

namespace WireMock.Http
{
    /// <summary>
    /// The ports.
    /// </summary>
    public static class Ports
    {
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
    }
}
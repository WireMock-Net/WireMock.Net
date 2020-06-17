using System.Collections.Generic;
using WireMock.Util;

namespace WireMock.Owin
{
    internal class HostUrlOptions
    {
        public ICollection<string> Urls { get; set; }

        public int? Port { get; set; }

        public bool UseSSL { get; set; }

        public ICollection<(string Url, int Port)> GetDetails()
        {
            var list = new List<(string Url, int Port)>();
            if (Urls == null)
            {
                int port = Port > 0 ? Port.Value : FindFreeTcpPort();
                list.Add(($"{(UseSSL ? "https" : "http")}://localhost:{port}", port));
            }
            else
            {
                foreach (string url in Urls)
                {
                    PortUtils.TryExtract(url, out string protocol, out string host, out int port);
                    list.Add((url, port));
                }
            }

            return list;
        }

        private int FindFreeTcpPort()
        {
#if USE_ASPNETCORE || NETSTANDARD2_0 || NETSTANDARD2_1
            return 0;
#else
            return PortUtils.FindFreeTcpPort();
#endif
        }
    }
}
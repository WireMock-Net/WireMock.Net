using System.Collections.Generic;
using WireMock.Util;

namespace WireMock.Owin;

internal class HostUrlOptions
{
    private const string LOCALHOST = "localhost";

    public ICollection<string>? Urls { get; set; }

    public int? Port { get; set; }

    public bool UseSSL { get; set; }

    public ICollection<HostUrlDetails> GetDetails()
    {
        var list = new List<HostUrlDetails>();
        if (Urls == null)
        {
            int port = Port > 0 ? Port.Value : FindFreeTcpPort();
            string protocol = UseSSL ? "https" : "http";
            list.Add(new HostUrlDetails { IsHttps = UseSSL, Url = $"{protocol}://{LOCALHOST}:{port}", Protocol = protocol, Host = LOCALHOST, Port = port });
        }
        else
        {
            foreach (string url in Urls)
            {
                if (PortUtils.TryExtract(url, out bool isHttps, out var protocol, out var host, out int port))
                {
                    list.Add(new HostUrlDetails { IsHttps = isHttps, Url = url, Protocol = protocol, Host = host, Port = port });
                }
            }
        }

        return list;
    }

    private static int FindFreeTcpPort()
    {
#if USE_ASPNETCORE || NETSTANDARD2_0 || NETSTANDARD2_1
        return 0;
#else
        return PortUtils.FindFreeTcpPort();
#endif
    }
}
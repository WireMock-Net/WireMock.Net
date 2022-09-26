using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Owin;

internal class HostUrlOptions
{
    private const string Localhost = "localhost";

    public ICollection<string>? Urls { get; set; }

    public int? Port { get; set; }

    public int? HttpsPort { get; set; }

    public HostingScheme HostingScheme { get; set; }

    public IReadOnlyList<HostUrlDetails> GetDetails()
    {
        var list = new List<HostUrlDetails>();
        if (Urls == null)
        {
            if (HostingScheme is HostingScheme.Http or HostingScheme.Https)
            {
                var port = Port > 0 ? Port.Value : FindFreeTcpPort();
                var scheme = HostingScheme == HostingScheme.Https ? "https" : "http";
                list.Add(new HostUrlDetails { IsHttps = HostingScheme == HostingScheme.Https, Url = $"{scheme}://{Localhost}:{port}", Scheme = scheme, Host = Localhost, Port = port });
            }

            if (HostingScheme == HostingScheme.HttpAndHttps)
            {
                var httpPort = Port > 0 ? Port.Value : FindFreeTcpPort();
                list.Add(new HostUrlDetails { IsHttps = false, Url = $"http://{Localhost}:{httpPort}", Scheme = "http", Host = Localhost, Port = httpPort });

                var httpsPort = FindFreeTcpPort(); // In this scenario, always get a free port for https.
                list.Add(new HostUrlDetails { IsHttps = true, Url = $"https://{Localhost}:{httpsPort}", Scheme = "https", Host = Localhost, Port = httpsPort });
            }
        }
        else
        {
            foreach (string url in Urls)
            {
                if (PortUtils.TryExtract(url, out var isHttps, out var protocol, out var host, out var port))
                {
                    list.Add(new HostUrlDetails { IsHttps = isHttps, Url = url, Scheme = protocol, Host = host, Port = port });
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
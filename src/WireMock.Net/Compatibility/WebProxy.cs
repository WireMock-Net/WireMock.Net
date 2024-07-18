// Copyright © WireMock.Net

#if NETSTANDARD1_3

// ReSharper disable once CheckNamespace
namespace System.Net;

internal class WebProxy : IWebProxy
{
    private readonly string _proxy;
    public ICredentials? Credentials { get; set; }

    public WebProxy(string proxy)
    {
        _proxy = proxy;
    }

    public Uri GetProxy(Uri destination)
    {
        return new Uri(_proxy);
    }

    public bool IsBypassed(Uri host)
    {
        return true;
    }
}
#endif
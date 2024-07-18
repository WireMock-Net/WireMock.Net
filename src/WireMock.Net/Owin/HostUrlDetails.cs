// Copyright © WireMock.Net

namespace WireMock.Owin;

/// <summary>
/// https://en.wikipedia.org/wiki/Uniform_Resource_Identifier
/// </summary>
internal struct HostUrlDetails
{
    public bool IsHttps { get; set; }

    public bool IsHttp2 { get; set; }

    public string Url { get; set; }
        
    public string Scheme { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }
}
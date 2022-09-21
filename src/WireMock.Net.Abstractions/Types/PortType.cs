using System;

namespace WireMock.Types;

[Flags]
public enum HostingProtocol
{
    None = 0x0,

    Http = 0x1,

    Https = 0x2,

    HttpAndHttps = Http | Https
}
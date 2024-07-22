// Copyright © WireMock.Net

using System;

namespace WireMock.Types;

[Flags]
public enum HostingScheme
{
    None = 0x0,

    Http = 0x1,

    Https = 0x2,

    HttpAndHttps = Http | Https
}
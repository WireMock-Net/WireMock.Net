using System;
using System.Collections.Generic;

namespace WireMock.Org.Abstractions
{
    /// <summary>
    /// Pre-emptive basic auth credentials to match against
    /// </summary>
    public class MappingsRequestBasicAuthCredentials
    {
        public string Password { get; set; }

        public string Username { get; set; }
    }
}
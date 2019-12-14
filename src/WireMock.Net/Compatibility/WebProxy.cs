#if NETSTANDARD1_3
using System;
using System.Net;

namespace System.Net
{
    internal class WebProxy : IWebProxy
    {
        private readonly string _proxy;
        public ICredentials Credentials { get; set; }

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
}
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireMock.Settings
{
    public interface IWebProxySettings
    {
        /// <summary>
        /// A string instance that contains the address of the proxy server.
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// The user name associated with the credentials.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// The password for the user name associated with the credentials.
        /// </summary>
        string Password { get; set; }
    }
}

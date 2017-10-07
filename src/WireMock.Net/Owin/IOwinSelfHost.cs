using System.Collections.Generic;
using System.Threading.Tasks;

namespace WireMock.Owin
{
    interface IOwinSelfHost
    {
        /// <summary>
        /// Gets a value indicating whether this server is started.
        /// </summary>
        /// <value>
        /// <c>true</c> if this server is started; otherwise, <c>false</c>.
        /// </value>
        bool IsStarted { get; }

        /// <summary>
        /// Gets the urls.
        /// </summary>
        /// <value>
        /// The urls.
        /// </value>
        List<string> Urls { get; }

        /// <summary>
        /// Gets the ports.
        /// </summary>
        /// <value>
        /// The ports.
        /// </value>
        List<int> Ports { get; }

        Task StartAsync();

        Task StopAsync();
    }
}
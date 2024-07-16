// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace WireMock.Owin;

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
    List<string> Urls { get; }

    /// <summary>
    /// Gets the ports.
    /// </summary>
    List<int> Ports { get; }

    /// <summary>
    /// The exception occurred when the host is running.
    /// </summary>
    Exception? RunningException { get; }

    Task StartAsync();

    Task StopAsync();
}
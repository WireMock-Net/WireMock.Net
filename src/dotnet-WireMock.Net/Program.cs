// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Server;

namespace WireMock.Net;

public class Program
{
    private static readonly int SleepTime = 30000;
    private static readonly ILogger XLogger = LoggerFactory.Create(o =>
    {
        o.SetMinimumLevel(LogLevel.Debug);
        o.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = false;
            options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss ";
        });
    }).CreateLogger("WireMock.Net");
    private static readonly IWireMockLogger Logger = new WireMockLogger(XLogger);

    private static WireMockServer _server = null!;

    static async Task Main(string[] args)
    {
        if (!StandAloneApp.TryStart(args, out _server!, Logger))
        {
            return;
        }

        Logger.Info("Press Ctrl+C to shut down");

        Console.CancelKeyPress += (s, e) =>
        {
            Stop("CancelKeyPress");
        };

        System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += ctx =>
        {
            Stop("AssemblyLoadContext.Default.Unloading");
        };

        while (true)
        {
            Logger.Info("Server running : {0}", _server.IsStarted);
            await Task.Delay(SleepTime).ConfigureAwait(false);
        }
    }

    private static void Stop(string why)
    {
        Logger.Info("Server stopping because '{0}'", why);
        _server.Stop();
        Logger.Info("Server stopped");
    }
}
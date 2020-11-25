using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WireMock.Net.StandAlone;
using WireMock.Server;

namespace WireMock
{
    public class Program
    {
        private static int SleepTime = 30000;
        private static readonly ILogger Logger = LoggerFactory.Create(o =>
        {
            o.SetMinimumLevel(LogLevel.Debug);
            o.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.UseUtcTimestamp = true;
                options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss ";
            });
        }).CreateLogger(string.Empty);

        private static WireMockServer Server;

        static async Task Main(string[] args)
        {
            Server = StandAloneApp.Start(args, new WireMockLogger(Logger));

            Logger.LogInformation("Press Ctrl+C to shut down");

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
                Logger.LogInformation("WireMock.Net server running : {IsStarted}", Server.IsStarted);
                await Task.Delay(SleepTime);
            }
        }

        private static void Stop(string why)
        {
            Logger.LogInformation("WireMock.Net server stopping because '{why}'", why);
            Server.Stop();
            Logger.LogInformation("WireMock.Net server stopped");
        }
    }
}
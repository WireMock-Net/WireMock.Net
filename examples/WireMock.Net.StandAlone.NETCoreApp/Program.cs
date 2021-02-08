using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using log4net.Config;
using log4net.Repository;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.StandAlone.NETCoreApp
{
    static class Program
    {
        private static readonly ILoggerRepository LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        // private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        private static int sleepTime = 30000;
        private static WireMockServer _server;

        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));

            if (!WireMockServerSettingsParser.TryParseArguments(args, out var settings, new WireMockLog4NetLogger()))
            {
                return;
            }

            settings.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            _server = WireMockServer.Start(settings);

            _server
                .Given(Request.Create()
                    .UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithTransformer()
                    .WithBody("{{Random Type=\"Integer\" Min=100 Max=999999}} {{Now}} {{Now \"yyyy-MMM\"}} {{Format (Now) \"MMM-dd\"}}"));

            Console.WriteLine($"{DateTime.UtcNow} Press Ctrl+C to shut down");

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
                Console.WriteLine($"{DateTime.UtcNow} WireMock.Net server running : {_server.IsStarted}");
                Thread.Sleep(sleepTime);
            }
        }

        private static void Stop(string why)
        {
            Console.WriteLine($"{DateTime.UtcNow} WireMock.Net server stopping because '{why}'");
            _server.Stop();
            Console.WriteLine($"{DateTime.UtcNow} WireMock.Net server stopped");
        }
    }
}
using System;
using System.ServiceProcess;
using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Server;
using WireMock.Settings;

namespace Wiremock.Net.Service
{
    public static class Program
    {
        #region Nested classes to support running as service
        public const string ServiceName = "Wiremock.Net.Service";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Start();
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        private static FluentMockServer _server;

        static void Main(string[] args)
        {
            // running as service
            if (!Environment.UserInteractive)
            {
                using (var service = new Service())
                {
                    ServiceBase.Run(service);
                }
            }
            else
            {
                // running as console app
                Start();

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        private static void Start()
        {
            _server = StandAloneApp.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://*:9091/" },
                StartAdminInterface = true,
                ReadStaticMappings = true,
                Logger = new WireMockConsoleLogger()
            });
        }

        private static void Stop()
        {
            _server.Stop();
        }
    }
}
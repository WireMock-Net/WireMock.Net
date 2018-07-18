using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.NETCoreApp
{
    static class Program
    {
        private static readonly ILoggerRepository LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(params string[] args)
        {
            XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));

            string url = "http://localhost:9999/";

            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { url },
                StartAdminInterface = true,
                Logger = new WireMockConsoleLogger()
            });
            System.Console.WriteLine("FluentMockServer listening at {0}", string.Join(",", server.Urls));

            server.SetBasicAuthentication("a", "b");

            server.AllowPartialMapping();

            server
                .Given(Request.Create()
                    .UsingGet()
                    .WithHeader("Keep-Alive-Test", "stef")
                )
                .RespondWith(Response.Create()
                    .WithHeader("Keep-Alive", "timeout=1, max=1")
                    .WithBody("Keep-Alive OK")
                );

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();

            System.Console.WriteLine("Displaying all requests");
            var allRequests = server.LogEntries;
            System.Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

            System.Console.WriteLine("Press any key to quit");
            System.Console.ReadKey();
        }
    }
}
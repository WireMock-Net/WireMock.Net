using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.Proxy.NETCoreApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTestDifferentPort().Wait(20000); // prints "1"
            RunTestDifferentPort().Wait(20000); // prints "1"

            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://localhost:9091", "https://localhost:9443" },
                StartAdminInterface = true,
                ReadStaticMappings = false,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "https://www.google.com",
                    //ClientX509Certificate2ThumbprintOrSubjectName = "www.yourclientcertname.com OR yourcertificatethumbprint (only if the service you're proxying to requires it)",
                    SaveMapping = true,
                    SaveMappingToFile = false,
                    BlackListedHeaders = new[] { "dnt", "Content-Length" }
                }
            });

            server.LogEntriesChanged += (sender, eventRecordArgs) =>
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(eventRecordArgs.NewItems, Formatting.Indented));
            };

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();
        }

        private static async Task RunTestDifferentPort()
        {
            var server = FluentMockServer.Start();

            server.Given(Request.Create().WithPath("/").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200).WithBody("Hello"));

            Thread.Sleep(1000);

            var response = await new HttpClient().GetAsync(server.Urls[0]);
            response.EnsureSuccessStatusCode();

            System.Console.WriteLine("RunTestDifferentPort - server.LogEntries.Count() = " + server.LogEntries.Count());

            server.Stop();
        }
    }
}

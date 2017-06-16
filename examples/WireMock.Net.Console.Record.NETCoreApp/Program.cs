using Newtonsoft.Json;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.Record.NETCoreApp
{
    static class Program
    {
        static void Main(params string[] args)
        {
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://localhost:9090/", "https://localhost:9096/" },
                StartAdminInterface = true,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "https://www.msn.com",
                    X509Certificate2ThumbprintOrSubjectName = "x3bwbapi-dev.nzlb.service.dev",
                    SaveMapping = true
                }
            });

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
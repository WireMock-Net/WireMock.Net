using Newtonsoft.Json;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.NETCoreApp
{
    static class Program
    {
        static void Main(params string[] args)
        {
            string url1 = "http://localhost:9095/";

            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { url1 },
                StartAdminInterface = true,
                ProxyAndRecordSettings = new ProxyAndRecordSettings { Url = "http://www.bbc.com" }
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
using Newtonsoft.Json;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.Proxy.NETCoreApp
{
    static class Program
    {
        static void Main(params string[] args)
        {
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://localhost:9091/", "https://localhost:9443/" },
                StartAdminInterface = true,
                ReadStaticMappings = false,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "https://www.google.com",
                    //ClientX509Certificate2ThumbprintOrSubjectName = "www.yourclientcertname.com OR yourcertificatethumbprint (only if the service you're proxying to requires it)",
                    SaveMapping = true,
                    SaveMappingToFile = false,
                    BlackListedHeaders = new [] { "dnt", "Content-Length" }
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
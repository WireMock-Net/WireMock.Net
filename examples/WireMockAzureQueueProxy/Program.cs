// Copyright © WireMock.Net

using Newtonsoft.Json;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace WireMockAzureQueueProxy;

static class Program
{
    static void Main(params string[] args)
    {
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new WireMockConsoleLogger(),
            Urls = new[] { "http://localhost:20001/" },
            StartAdminInterface = false,
            ReadStaticMappings = true,
            WatchStaticMappings = true,
            WatchStaticMappingsInSubdirectories = true,
            //ProxyAndRecordSettings = new ProxyAndRecordSettings
            //{
            //    Url = "http://127.0.0.1:10001",
            //    SaveMapping = true,
            //    SaveMappingToFile = true,
            //    AppendGuidToSavedMappingFile = true
            //}
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
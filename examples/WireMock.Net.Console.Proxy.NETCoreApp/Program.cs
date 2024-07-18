// Copyright © WireMock.Net

using Newtonsoft.Json;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.Proxy.NETCoreApp;

static class Program
{
    static void Main(params string[] args)
    {
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = new WireMockConsoleLogger(),
            Urls = new[] { "http://localhost:9091/", "https://localhost:9443/" },
            StartAdminInterface = true,
            ReadStaticMappings = true,
            WatchStaticMappings = true,
            WatchStaticMappingsInSubdirectories = true,
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                Url = "http://postman-echo.com/post",
                SaveMapping = true,
                SaveMappingToFile = true,
                ExcludedHeaders = new[] { "Postman-Token" },
                ExcludedCookies = new[] { "sails.sid" }
            }
        });

        //server
        //    .Given(Request.Create().UsingGet())
        //    .RespondWith(Response.Create()
        //       .WithProxy(new ProxyAndRecordSettings
        //       {
        //           Url = "http://postman-echo.com/post",
        //           SaveMapping = true,
        //           SaveMappingToFile = true
        //       }));

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
using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.OpenApiParser.ConsoleApp
{
    public static class Run
    {
        public static void RunServer(IEnumerable<MappingModel> mappings)
        {
            string url1 = "http://localhost:9091/";

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                AllowCSharpCodeMatcher = true,
                Urls = new[] { url1 },
                StartAdminInterface = true,
                ReadStaticMappings = false,
                WatchStaticMappings = false,
                WatchStaticMappingsInSubdirectories = false,
                Logger = new WireMockConsoleLogger(),
            });
            Console.WriteLine("WireMockServer listening at {0}", string.Join(",", server.Urls));

            server.SetBasicAuthentication("a", "b");

            server.WithMappingModel(mappings.ToArray());

            Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();
        }
    }
}
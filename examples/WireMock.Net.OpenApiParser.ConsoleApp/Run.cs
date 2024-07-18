// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Logging;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.OpenApiParser.ConsoleApp;

public static class Run
{
    public static WireMockServer RunServer(
        string path,
        string url,
        bool dynamicExamples = true,
        IWireMockOpenApiParserExampleValues? examplesValuesGenerator = null,
        ExampleValueType pathPatternToUse = ExampleValueType.Wildcard,
        ExampleValueType headerPatternToUse = ExampleValueType.Wildcard
    )
    {
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            AllowCSharpCodeMatcher = true,
            Urls = new[] { url },
            StartAdminInterface = true,
            ReadStaticMappings = true,
            WatchStaticMappings = false,
            WatchStaticMappingsInSubdirectories = false,
            Logger = new WireMockConsoleLogger(),
            SaveUnmatchedRequests = true
        });

        Console.WriteLine("WireMockServer listening at {0}", string.Join(",", server.Urls));

        //server.SetBasicAuthentication("a", "b");

        var settings = new WireMockOpenApiParserSettings
        {
            DynamicExamples = dynamicExamples,
            ExampleValues = examplesValuesGenerator,
            PathPatternToUse = pathPatternToUse,
            HeaderPatternToUse = headerPatternToUse,
        };

        server.WithMappingFromOpenApiFile(path, settings, out var diag);

        return server;
    }

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

        server.WithMapping(mappings.ToArray());

        Console.WriteLine("Press any key to stop the server");
        System.Console.ReadKey();
        server.Stop();
    }
}
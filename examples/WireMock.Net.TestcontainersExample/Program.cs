// Copyright © WireMock.Net

using Newtonsoft.Json;
using Testcontainers.MsSql;
using WireMock.Net.Testcontainers;

namespace WireMock.Net.TestcontainersExample;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var container = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword("x", "y")
            .WithMappings(@"C:\Dev\GitHub\WireMock.Net\examples\WireMock.Net.Console.NET6\__admin\mappings")
            .WithWatchStaticMappings(true)
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        var logs = await container.GetLogsAsync(DateTime.Now.AddDays(-1)).ConfigureAwait(false);
        Console.WriteLine("logs = " + logs.Stdout);

        var restEaseApiClient = container.CreateWireMockAdminClient();

        var settings = await restEaseApiClient.GetSettingsAsync();
        Console.WriteLine("settings = " + JsonConvert.SerializeObject(settings, Formatting.Indented));

        var mappings = await restEaseApiClient.GetMappingsAsync();
        Console.WriteLine("mappings = " + JsonConvert.SerializeObject(mappings, Formatting.Indented));

        var client = container.CreateClient();
        var result = await client.GetStringAsync("/static/mapping");
        Console.WriteLine("result = " + result);

        await container.StopAsync();

        var sql = new MsSqlBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();
    }
}
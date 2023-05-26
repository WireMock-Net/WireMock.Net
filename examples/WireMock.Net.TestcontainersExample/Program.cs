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
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        var logs = await container.GetLogsAsync().ConfigureAwait(false);
        Console.WriteLine("logs = " + logs.Stdout);

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic eDp5");

        var settingsUri = container.GetPublicUrl() + "__admin/settings";
        var settings = await httpClient.GetStringAsync(settingsUri).ConfigureAwait(false);

        Console.WriteLine("settings = " + settings);

        var mappingsUri = container.GetPublicUrl() + "__admin/mappings";
        var mappings = await httpClient.GetStringAsync(mappingsUri).ConfigureAwait(false);

        Console.WriteLine("mappings = " + mappings);

        await container.StopAsync();

        var sql = new MsSqlBuilder().Build();
    }
}
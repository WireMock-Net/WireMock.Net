using WireMock.Net.Testcontainers;

namespace WireMock.Net.TestcontainersExample;

internal class Program
{
    static async Task Main(string[] args)
    {
        var container = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword("x", "y")
            .WithReadStaticMappings()
            .WithAllowPartialMapping()
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        var logs = await container.GetLogsAsync(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
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
    }
}
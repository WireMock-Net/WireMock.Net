using WireMock.Net.Testcontainers;

namespace WireMock.Net.TestcontainersExample;

internal class Program
{
    static async Task Main(string[] args)
    {
        var container = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword("x", "y")
            .Build();

        await container.StartAsync().ConfigureAwait(false);

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
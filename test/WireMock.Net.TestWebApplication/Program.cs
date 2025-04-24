using WireMock.Net.AspNetCore.Middleware;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace WireMock.Net.TestWebApplication;

// Make the implicit Program class public so test projects can access it.
public class Program
{
    public static async Task Main(string[] args)
    {
        var alwaysRedirectToWireMock = args.Contains("--AlwaysRedirectToWireMock=true");

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<TaskQueue>();
        builder.Services.AddHostedService<TestBackgroundService>();

        builder.Services.AddWireMockService(server =>
        {
            server.Given(Request.Create()
                .WithPath("/test1")
                .UsingAnyMethod()
            ).RespondWith(Response.Create()
                .WithBody("Hello 1 from WireMock.Net !")
            );

            server.Given(Request.Create()
                .WithPath("/test2")
                .UsingAnyMethod()
            ).RespondWith(Response.Create()
                .WithBody("Hello 2 from WireMock.Net !")
            );

            server.Given(Request.Create()
                .WithPath("/test3")
                .UsingAnyMethod()
            ).RespondWith(Response.Create()
                .WithBody("Hello 3 from WireMock.Net !")
            );
        }, alwaysRedirectToWireMock);

        var app = builder.Build();

        app.MapGet("/real1", async (HttpClient client) =>
        {
            var result = await client.GetStringAsync("https://real-api:12345/test1");
            return result;
        });

        app.MapGet("/real2", async (IHttpClientFactory factory) =>
        {
            using var client = factory.CreateClient();
            return await client.GetStringAsync("https://real-api:12345/test2");
        });

        app.MapGet("/real3", async (TaskQueue taskQueue, CancellationToken cancellationToken) =>
        {
            return await taskQueue.Enqueue("https://real-api:12345/test3", cancellationToken);
        });

        await app.RunAsync();
    }
}
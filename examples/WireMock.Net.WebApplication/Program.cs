using WireMock.Net.AspNetCore.Middleware;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsProduction())
{
    builder.Services.AddWireMockService(server =>
    {
        server.Given(Request.Create()
            .WithPath("/test1")
            .UsingAnyMethod()
        ).RespondWith(Response.Create()
            .WithBody("1 : WireMock.Net !")
        );

        server.Given(Request.Create()
            .WithPath("/test2")
            .UsingAnyMethod()
        ).RespondWith(Response.Create()
            .WithBody("2 : WireMock.Net !")
        );
    }, true);
}

var app = builder.Build();

app.MapGet("/weatherforecast", async (HttpClient client) =>
{
    var result = await client.GetStringAsync("https://real-api:12345/test1");

    return Enumerable.Range(1, 3).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            result
        ));
});

app.MapGet("/weatherforecast2", async (IHttpClientFactory factory) =>
{
    using var client = factory.CreateClient();
    var result = await client.GetStringAsync("https://real-api:12345/test2");

    return Enumerable.Range(1, 3).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            result
        ));
});

await app.RunAsync();
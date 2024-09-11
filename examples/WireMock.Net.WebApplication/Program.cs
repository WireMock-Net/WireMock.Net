using WireMock.Net.AspNetCore.Middleware;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsProduction())
{
    builder.Services.AddWireMockService(server =>
    {
        server.Given(Request.Create()
            .WithPath("/test")
            .UsingAnyMethod()
        ).RespondWith(Response.Create()
            .WithBody("WebApplication")
        );
    }, true);
}

var app = builder.Build();

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                "..."
            ))
        .ToArray();
    return forecast;
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
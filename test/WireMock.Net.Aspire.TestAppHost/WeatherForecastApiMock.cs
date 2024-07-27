using WireMock.Client.Builders;

namespace WireMock.Net.Aspire.TestAppHost;

internal class WeatherForecastApiMock
{
    public static async Task BuildAsync(AdminApiMappingBuilder builder)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        builder.Given(b => b
            .WithRequest(request => request
                .UsingGet()
                .WithPath("/weatherforecast2")
            )
            .WithResponse(response => response
                .WithHeaders(h => h.Add("Content-Type", "application/json"))
                .WithBodyAsJson(() => Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray())
            )
        );

        await builder.BuildAndPostAsync();
    }
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
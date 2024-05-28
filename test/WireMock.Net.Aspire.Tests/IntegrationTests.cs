using System.Net.Http.Json;
using FluentAssertions;
using Projects;
using Xunit.Abstractions;

namespace WireMock.Net.Aspire.Tests;

public class IntegrationTests(ITestOutputHelper output)
{
    private record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);

    [Fact]
    public async Task StartAppHostWithWireMockAndCreateHttpClientToCallTheMockedWeatherForecastEndpoint()
    {
        if (!DockerUtils.IsDockerRunningLinuxContainerMode())
        {
            output.WriteLine("Docker is not running in Linux container mode. Skipping test.");
            return;
        }

        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<WireMock_Net_Aspire_TestAppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        // Act
        using var httpClient = app.CreateHttpClient("wiremock-service");
        var weatherForecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert
        weatherForecasts.Should().BeEquivalentTo(new[]
        {
            new WeatherForecast(new DateOnly(2024, 5, 24), -10, "Freezing"),
            new WeatherForecast(new DateOnly(2024, 5, 25), +33, "Hot")
        });
    }

    [Fact]
    public async Task StartAppHostWithWireMockAndCreateWireMockAdminClientToCallTheAdminEndpoint()
    {
        if (!DockerUtils.IsDockerRunningLinuxContainerMode())
        {
            output.WriteLine("Docker is not running in Linux container mode. Skipping test.");
            return;
        }

        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<WireMock_Net_Aspire_TestAppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        // Act
        var adminClient = app.CreateWireMockAdminClient("wiremock-service");
        var settings = await adminClient.GetSettingsAsync();

        // Assert
        settings.Should().NotBeNull();
    }
}
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

        // Wait 2 seconds to ensure mappings are created via the MappingBuilder
        await Task.Delay(2000);

        using var httpClient = app.CreateHttpClient("wiremock-service");

        // Act 1
        var weatherForecasts1 = await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert 1
        weatherForecasts1.Should().BeEquivalentTo(new[]
        {
            new WeatherForecast(new DateOnly(2024, 5, 24), -10, "Freezing"),
            new WeatherForecast(new DateOnly(2024, 5, 25), +33, "Hot")
        });

        // Act 2
        var weatherForecasts2 = await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast2");

        // Assert 2
        weatherForecasts2.Should().HaveCount(5);
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

        // Wait 2 seconds to ensure mappings are created via the MappingBuilder
        await Task.Delay(2000);

        var adminClient = app.CreateWireMockAdminClient("wiremock-service");

        // Act 1
        var settings = await adminClient.GetSettingsAsync();

        // Assert 1
        settings.Should().NotBeNull();
        
        // Act 2
        var mappings = await adminClient.GetMappingsAsync();

        // Assert 2
        mappings.Should().HaveCount(2);
    }
}
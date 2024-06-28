using WireMock.Net.Aspire.TestAppHost;

var builder = DistributedApplication.CreateBuilder(args);

var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings");

builder
    .AddWireMock("wiremock-service")
    .WithAdminUserNameAndPassword($"user-{Guid.NewGuid()}", $"pwd-{Guid.NewGuid()}")
    .WithMappingsPath(mappingsPath)
    .WithWatchStaticMappings()
    .WithApiMappingBuilder(WeatherForecastApiMock.BuildAsync);

await builder
    .Build()
    .RunAsync();
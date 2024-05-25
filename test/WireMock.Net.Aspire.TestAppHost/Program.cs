var builder = DistributedApplication.CreateBuilder(args);

var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings");

builder
    .AddWireMock("wiremock-service")
    .WithMappingsPath(mappingsPath)
    .WithWatchStaticMappings();

await builder.Build().RunAsync();
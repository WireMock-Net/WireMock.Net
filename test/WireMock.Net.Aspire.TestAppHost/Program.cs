var builder = DistributedApplication.CreateBuilder(args);

var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings");

builder
    .AddWireMock("wiremock-service")
    .WithMappingsPath(mappingsPath)
    .WithReadStaticMappings();

await builder.Build().RunAsync();
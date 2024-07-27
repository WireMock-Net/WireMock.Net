using AspireApp1.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

//IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");

var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings");

Console.WriteLine($"MappingsPath: {mappingsPath}");

var wiremock = builder
    .AddWireMock("apiservice", WireMockServerArguments.DefaultPort)
    .WithMappingsPath(mappingsPath)
    .WithReadStaticMappings()
    .WithApiMappingBuilder(WeatherForecastApiMock.BuildAsync);

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(wiremock);

builder.Build().Run();
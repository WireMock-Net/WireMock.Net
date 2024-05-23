var builder = DistributedApplication.CreateBuilder(args);

//IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");

var wiremock = builder
    .AddWireMock("apiservice")
    .WithMappingsPath(Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings"))
    .WithReadStaticMappings()
    .WithWatchStaticMappings();

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(wiremock);

builder.Build().Run();
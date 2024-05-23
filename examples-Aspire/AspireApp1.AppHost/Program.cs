var builder = DistributedApplication.CreateBuilder(args);

//IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");

var arguments = new WireMockServerArguments
{
    ReadStaticMappings = true,
    MappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings")
};
var wiremock = builder.AddWireMock("apiservice", arguments);

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(wiremock);

builder.Build().Run();
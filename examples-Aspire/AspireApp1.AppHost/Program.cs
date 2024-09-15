using AspireApp1.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

//IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.AspireApp1_ApiService>("apiservice");

var mappingsPath = Path.Combine(Directory.GetCurrentDirectory(), "WireMockMappings");

var apiService = builder
    .AddWireMock("apiservice", WireMockServerArguments.DefaultPort)
    .WithMappingsPath(mappingsPath)
    .WithReadStaticMappings()
    .WithApiMappingBuilder(WeatherForecastApiMock.BuildAsync);

var apiService2 = builder
    .AddWireMock("apiservice", WireMockServerArguments.DefaultPort)
    .WithApiMappingBuilder(adminApiBuilder =>
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        adminApiBuilder.Given(b => b
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

        return Task.CompletedTask;
    });

builder.AddProject<Projects.AspireApp1_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
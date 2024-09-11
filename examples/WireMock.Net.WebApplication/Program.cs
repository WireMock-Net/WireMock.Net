using WireMock.Net.AspNetCore.Middleware;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

if (!builder.Environment.IsProduction())
{
    builder.Services.AddWireMockService(server =>
    {
        server.Given(Request.Create()
            .WithPath("/test")
            .UsingAnyMethod()
        ).RespondWith(Response.Create()
            .WithBody("WebApplication")
        );
    }, true);
}

var app = builder.Build();

app.MapControllers();

await app.RunAsync();
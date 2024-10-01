// Copyright © WireMock.Net

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WireMock.Net.Middleware.Tests;

internal class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    private readonly List<(string Key, string Value)> _settings = new();

    public CustomWebApplicationFactory(bool alwaysRedirectToWireMock = true)
    {
        _settings.Add(("AlwaysRedirectToWireMock", alwaysRedirectToWireMock.ToString().ToLowerInvariant()));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        foreach (var arg in _settings)
        {
            builder.UseSetting(arg.Key, arg.Value);
        }
    }
}
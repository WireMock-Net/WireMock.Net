// Copyright © WireMock.Net

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Stef.Validation;
using WireMock.Net.AspNetCore.Middleware.HttpDelegatingHandler;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.AspNetCore.Middleware;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all the components necessary to run WireMock.Net as a background service.
    /// </summary>
    public static IServiceCollection AddWireMockService(
        this IServiceCollection services,
        Action<WireMockServer> configure,
        bool alwaysRedirectToWireMock = true,
        WireMockServerSettings? settings = null
    )
    {
        Guard.NotNull(services);
        Guard.NotNull(configure);

        services.AddTransient<WireMockDelegationHandler>();

        services.AddSingleton(new WireMockServerInstance(configure, settings));

        services.AddSingleton(new WireMockDelegationHandlerSettings
        {
            AlwaysRedirect = alwaysRedirectToWireMock
        });

        services.AddHostedService<WireMockBackgroundService>();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.ConfigureAll<HttpClientFactoryOptions>(options =>
        {
            options.HttpMessageHandlerBuilderActions.Add(builder =>
            {
                builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<WireMockDelegationHandler>());
            });
        });

        return services;
    }
}
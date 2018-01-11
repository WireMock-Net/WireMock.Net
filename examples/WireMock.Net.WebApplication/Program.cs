using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WireMock.Settings;

namespace WireMock.Net.WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Run app
            serviceProvider.GetService<App>().Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // <-- this is needed to to override settings via the Azure Portal App Settings
                .Build();

            // Add LoggerFactory
            var factory = new LoggerFactory();
            serviceCollection.AddSingleton(factory
                .AddConsole(configuration.GetSection("Logging"))
                .AddDebug()
                .AddAzureWebAppDiagnostics()
            );

            serviceCollection.AddSingleton(factory.CreateLogger("WireMock.Net Logger"));

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(configuration);

            // Add access to IFluentMockServerSettings
            var settings = configuration.GetSection("FluentMockServerSettings").Get<FluentMockServerSettings>();
            serviceCollection.AddSingleton<IFluentMockServerSettings>(settings);

            // Add services
            serviceCollection.AddTransient<IWireMockService, WireMockService>();

            // Add app
            serviceCollection.AddTransient<App>();
        }
    }
}
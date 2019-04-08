using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WireMock.Logging;
using WireMock.Settings;

namespace WireMock.Net.WebApp
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

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // <-- this is needed to to override settings via the Azure Portal App Settings
                .Build();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
            });

            // Add access to IFluentMockServerSettings
            var settings = configuration.GetSection("FluentMockServerSettings").Get<FluentMockServerSettings>();
            services.AddSingleton<IFluentMockServerSettings>(settings);

            // Add services
            services.AddTransient<IWireMockLogger, WireMockLogger>();
            services.AddTransient<IWireMockService, WireMockService>();

            // Add app
            services.AddTransient<App>();
        }
    }
}
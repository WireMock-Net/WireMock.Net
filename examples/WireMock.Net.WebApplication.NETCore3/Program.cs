using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WireMock.Settings;
// using Microsoft.Extensions.Logging.Console;

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
            //var consoleLog = new ConsoleLoggerProvider(configuration.GetSection("Logging"));
            serviceCollection.AddSingleton(factory
              //  .AddProvider(configuration.GetSection("Logging"))
//                .AddDebug()
  //              .AddAzureWebAppDiagnostics()
            );

            serviceCollection.AddSingleton(factory.CreateLogger("WireMock.Net Logger"));

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton(configuration);

            // Add access to WireMockServerSettings
            var settings = configuration.GetSection("WireMockServerSettings").Get<WireMockServerSettings>();
            serviceCollection.AddSingleton<WireMockServerSettings>(settings);

            // Add services
            serviceCollection.AddTransient<IWireMockService, WireMockService>();

            // Add app
            serviceCollection.AddTransient<App>();
        }
    }
}
#if USE_ASPNETCORE && !NETSTANDARD1_3
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WireMock.Owin
{
    internal partial class AspNetCoreSelfHost
    {
        private static void SetKestrelOptionsLimits(KestrelServerOptions options)
        {
            options.Limits.MaxRequestBodySize = null; // https://stackoverflow.com/questions/46738364/increase-upload-request-length-limit-in-kestrel
            options.Limits.MaxRequestBufferSize = null;
            options.Limits.MaxRequestHeaderCount = 100;
            options.Limits.MaxResponseBufferSize = null;
        }

        private static void SetHttpsAndUrls(KestrelServerOptions options, ICollection<(string Url, int Port)> urlDetails)
        {
            foreach (var detail in urlDetails)
            {
                if (detail.Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    options.Listen(System.Net.IPAddress.Any, detail.Port, listenOptions =>
                    {
                        listenOptions.UseHttps();
                    });
                }
                else
                {
                    options.Listen(System.Net.IPAddress.Any, detail.Port);
                }
            }
        }
    }

    internal static class IWebHostBuilderExtensions
    {
        internal static IWebHostBuilder ConfigureAppConfigurationUsingEnvironmentVariables(this IWebHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration(config =>
            {
                config.AddEnvironmentVariables();
            });
        }

        internal static IWebHostBuilder ConfigureKestrelServerOptions(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices((context, services) =>
            {
                services.Configure<KestrelServerOptions>(context.Configuration.GetSection("Kestrel"));
            });
        }
    }
}
#endif
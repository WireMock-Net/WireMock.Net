#if USE_ASPNETCORE && NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.HttpsCertificate;

namespace WireMock.Owin
{
    internal partial class AspNetCoreSelfHost
    {
        private static void SetKestrelOptionsLimits(KestrelServerOptions options)
        {
            options.Limits.MaxRequestBufferSize = null;
            options.Limits.MaxRequestHeaderCount = 100;
            options.Limits.MaxResponseBufferSize = null;
        }

        private static void SetHttpsAndUrls(KestrelServerOptions options, IWireMockMiddlewareOptions wireMockMiddlewareOptions, IEnumerable<HostUrlDetails> urlDetails)
        {
            if (urlDetails.Any(u => u.IsHttps))
            {
                if (wireMockMiddlewareOptions.CustomCertificateDefined)
                {

                }
                else
                {
                    options.UseHttps(PublicCertificateHelper.GetX509Certificate2());
                }
            }
        }
    }

    internal static class IWebHostBuilderExtensions
    {
        internal static IWebHostBuilder ConfigureAppConfigurationUsingEnvironmentVariables(this IWebHostBuilder builder) => builder;

        internal static IWebHostBuilder ConfigureKestrelServerOptions(this IWebHostBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            return builder.ConfigureServices(services =>
            {
                services.Configure<KestrelServerOptions>(configuration.GetSection("Kestrel"));
            });
        }
    }
}
#endif
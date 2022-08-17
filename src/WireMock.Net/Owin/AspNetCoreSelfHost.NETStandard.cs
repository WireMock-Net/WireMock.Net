#if USE_ASPNETCORE && !NETSTANDARD1_3
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WireMock.HttpsCertificate;

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

        private static void SetHttpsAndUrls(KestrelServerOptions kestrelOptions, IWireMockMiddlewareOptions wireMockMiddlewareOptions, IEnumerable<HostUrlDetails> urlDetails)
        {
            foreach (var urlDetail in urlDetails)
            {
                if (urlDetail.IsHttps)
                {
                    kestrelOptions.ListenAnyIP(urlDetail.Port, listenOptions =>
                    {
                        if (wireMockMiddlewareOptions.CustomCertificateDefined)
                        {
                            listenOptions.UseHttps(CertificateLoader.LoadCertificate(
                                wireMockMiddlewareOptions.X509StoreName,
                                wireMockMiddlewareOptions.X509StoreLocation,
                                wireMockMiddlewareOptions.X509ThumbprintOrSubjectName,
                                wireMockMiddlewareOptions.X509CertificateFilePath,
                                wireMockMiddlewareOptions.X509CertificatePassword,
                                urlDetail.Host)
                            );
                        }
                        else
                        {
                            listenOptions.UseHttps();
                        }
                    });
                }
                else
                {
                    kestrelOptions.ListenAnyIP(urlDetail.Port);
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
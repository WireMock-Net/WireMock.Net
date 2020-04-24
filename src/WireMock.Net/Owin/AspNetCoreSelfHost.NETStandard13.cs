#if USE_ASPNETCORE && NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel;
using WireMock.HttpsCertificate;

namespace WireMock.Owin
{
    internal partial class AspNetCoreSelfHost
    {
        private static void SetKestrelOptionsLimits(KestrelServerOptions options)
        {
            options.Limits.KeepAliveTimeout = TimeSpan.MaxValue;
            options.Limits.MaxRequestBufferSize = null;
            options.Limits.MaxRequestHeaderCount = 100;
            options.Limits.MaxResponseBufferSize = null;
            options.Limits.RequestHeadersTimeout = TimeSpan.MaxValue;
        }

        private static void SetHttpsAndUrls(KestrelServerOptions options, ICollection<(string Url, int Port)> urlDetails)
        {
            var urls = urlDetails.Select(u => u.Url);
            if (urls.Any(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                options.UseHttps(PublicCertificateHelper.GetX509Certificate2());
            }
        }
    }
}
#endif
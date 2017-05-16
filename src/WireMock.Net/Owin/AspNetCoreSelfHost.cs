#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class AspNetCoreSelfHost : IOwinSelfHost
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly WireMockMiddlewareOptions _options;
        private readonly string[] _uriPrefixes;

        public bool IsStarted { get; private set; }

        public List<Uri> Urls { get; } = new List<Uri>();

        public List<int> Ports { get; } = new List<int>();

        public AspNetCoreSelfHost([NotNull] WireMockMiddlewareOptions options, [NotNull] params string[] uriPrefixes)
        {
            Check.NotNull(options, nameof(options));
            Check.NotEmpty(uriPrefixes, nameof(uriPrefixes));

            foreach (string uriPrefix in uriPrefixes)
            {
                var uri = new Uri(uriPrefix);
                Urls.Add(uri);
                Ports.Add(uri.Port);
            }

            _options = options;
            _uriPrefixes = uriPrefixes;
        }

        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                var host = new WebHostBuilder()
                    .ConfigureLogging(factory => factory.AddConsole(LogLevel.None))
                    .Configure(appBuilder =>
                    {
                        // appBuilder.UseExceptionHandler(builder => )
                        appBuilder.UseMiddleware<WireMockMiddleware>(_options);
                    })
                    .UseKestrel()
                    .UseUrls(_uriPrefixes)
                    .Build();

                host.Run(_cts.Token);

                IsStarted = true;
            }, _cts.Token);
        }

        public Task StopAsync()
        {
            _cts.Cancel();

            IsStarted = false;

            return Task.FromResult(true);
        }
    }

    internal class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables();
            //Configuration = builder.Build();
        }

        // public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<WireMockMiddleware>();
        }
    }
}
#endif
using Owin;

namespace WireMock.Owin
{
    internal static class WireMockMiddlewareExtensions
    {
        public static IAppBuilder UseWireMockMiddleware(this IAppBuilder app, WireMockMiddlewareOptions options)
        {
            app.Use<WireMockMiddleware>(options);

            return app;
        }
    }
}
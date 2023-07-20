using System.Net.Http;

namespace WireMock.Http;

// https://github.com/WireMock-Net/WireMock.Net/issues/974
#if !NETSTANDARD1_3
extern alias SystemNetHttpFormatting;
#endif

internal static class HttpClientFactory2
{
    public static HttpClient Create(params DelegatingHandler[] handlers)
    {
#if NETSTANDARD1_3
        return new HttpClient();
#else
        // ReSharper disable once RedundantNameQualifier
        return SystemNetHttpFormatting::System.Net.Http.HttpClientFactory.Create(handlers);
#endif
    }

    public static HttpClient Create(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
#if NETSTANDARD1_3
        return new HttpClient(innerHandler);
#else
        // ReSharper disable once RedundantNameQualifier
        return SystemNetHttpFormatting::System.Net.Http.HttpClientFactory.Create(innerHandler, handlers);
#endif
    }
}
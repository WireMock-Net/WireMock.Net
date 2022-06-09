using System.Net.Http;

namespace WireMock.Http;

internal static class HttpClientFactory2
{
    public static HttpClient Create(params DelegatingHandler[] handlers)
    {
#if NETSTANDARD1_3
        return new HttpClient();
#else
        return HttpClientFactory.Create(handlers);
#endif
    }

    public static HttpClient Create(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
#if NETSTANDARD1_3
        return new HttpClient(innerHandler);
#else
        return HttpClientFactory.Create(innerHandler, handlers);
#endif
    }
}
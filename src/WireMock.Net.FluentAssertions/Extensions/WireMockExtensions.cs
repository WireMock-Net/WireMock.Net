using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions
{
    public static class WireMockExtensions
    {
        public static WireMockReceivedAssertions Should(this IWireMockServer instance)
        {
            return new WireMockReceivedAssertions(instance);
        }
    }
}
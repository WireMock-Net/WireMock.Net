using WireMock.Settings;

namespace WireMock.Server
{
    public class FluentMockServer : WireMockServer
    {
        public FluentMockServer(IFluentMockServerSettings settings) : base((WireMockServerSettings) settings)
        {
        }
    }
}
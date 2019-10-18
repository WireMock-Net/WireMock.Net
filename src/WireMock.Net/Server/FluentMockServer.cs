using WireMock.Settings;

namespace WireMock.Server
{
    public class FluentMockServer : WireMockServer
    {
        private FluentMockServer(WireMockServerSettings settings) : base(settings)
        {
        }
    }
}
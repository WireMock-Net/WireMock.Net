using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions
{
    public class WireMockANumberOfCallsAssertions
    {
        private readonly WireMockServer _server;
        private readonly int _callsCount;

        public WireMockANumberOfCallsAssertions(WireMockServer server, int callsCount)
        {
            _server = server;
            _callsCount = callsCount;
        }

        public WireMockAssertions Calls()
        {
            return new WireMockAssertions(_server, _callsCount);
        }
    }
}
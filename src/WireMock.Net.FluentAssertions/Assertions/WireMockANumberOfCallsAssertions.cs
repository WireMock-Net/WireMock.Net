using WireMock.Server;

namespace WireMock.FluentAssertions
{
    // ReSharper disable once CheckNamespace
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
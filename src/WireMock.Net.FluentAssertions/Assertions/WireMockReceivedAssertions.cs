using FluentAssertions.Primitives;
using WireMock.Server;

// ReSharper disable once CheckNamespace
namespace WireMock.FluentAssertions
{
    public class WireMockReceivedAssertions : ReferenceTypeAssertions<WireMockServer, WireMockReceivedAssertions>
    {
        public WireMockReceivedAssertions(WireMockServer server)
        {
            Subject = server;
        }
        
        public WireMockAssertions HaveReceivedACall()
        {
            return new WireMockAssertions(Subject, null);
        }
        
        public WireMockANumberOfCallsAssertions HaveReceived(int callsCount)
        {
            return new WireMockANumberOfCallsAssertions(Subject, callsCount);
        }

        protected override string Identifier => "wiremockserver";
    }
}
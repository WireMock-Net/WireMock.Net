using System;
using NFluent;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests
{
    public class RequestWithUrlTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public void Request_WithUrl()
        {
            // given
            var spec = Request.Create().WithUrl("*/foo");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "blabla", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithUrlExact()
        {
            // given
            var spec = Request.Create().WithUrl("http://localhost/foo");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "blabla", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }
    }
}
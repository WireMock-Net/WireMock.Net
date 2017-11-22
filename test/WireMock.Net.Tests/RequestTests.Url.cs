using System;
using NFluent;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests
{
    public partial class RequestTests
    {
        [Fact]
        public void Should_specify_requests_matching_given_url_wildcard()
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
        public void Should_specify_requests_matching_given_url_exact()
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
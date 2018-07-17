using System;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
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
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "blabla", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithUrl_WildcardMatcher()
        {
            // given
            var spec = Request.Create().WithUrl(new WildcardMatcher("*/foo"));

            // when
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "blabla", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithUrl_Func()
        {
            // given
            var spec = Request.Create().WithUrl(url => url.Contains("foo"));

            // when
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "blabla", ClientIp);

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }
    }
}
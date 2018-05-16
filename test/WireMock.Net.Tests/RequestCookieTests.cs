using System;
using System.Collections.Generic;
using NFluent;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests
{
    public class RequestCookieTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public void Request_WithCookie_OK()
        {
            // given
            var spec = Request.Create().UsingAnyMethod().WithCookie("session", "a*");

            // when
            var request = new RequestMessage(new Uri("http://localhost/foo"), "PUT", ClientIp, null, null, new Dictionary<string, string> { { "session", "abc" } });

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }
    }
}
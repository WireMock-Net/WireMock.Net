using System;
using NFluent;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests
{
    //[TestFixture]
    public partial class RequestTests
    {
        [Fact]
        public void Request_WithClientIP_Match_Ok()
        {
            // given
            var spec = Request.Create().WithClientIP("127.0.0.2", "1.1.1.1");

            // when
            var request = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.2");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(1.0);
        }

        [Fact]
        public void Request_WithClientIP_Match_Fail()
        {
            // given
            var spec = Request.Create().WithClientIP("127.0.0.2");

            // when
            var request = new RequestMessage(new Uri("http://localhost"), "GET", "192.1.1.1");

            // then
            var requestMatchResult = new RequestMatchResult();
            Check.That(spec.GetMatchingScore(request, requestMatchResult)).IsEqualTo(0.0);
        }
    }
}
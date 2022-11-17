using FluentAssertions;
using WireMock.Matchers;
using Xunit;
using WireMock.RequestBuilders;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Net.Tests
{
    public class RequestWithUrlTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public void Request_WithUrl_Regex()
        {
            // Assign
            var spec = Request.Create().WithUrl(new RegexMatcher("(some\\/service\\/v1\\/name)([?]{1})(param.source=SYSTEM){1}([&]{1})(param.id=123457890){1}$")).UsingAnyMethod();

            // Act
            var body = new BodyData();
            var request = new RequestMessage(new UrlDetails("https://localhost/some/service/v1/name?param.source=SYSTEM&param.id=123457890"), "POST", ClientIp, body);

            // Assert
            var requestMatchResult = new RequestMatchResult();
            spec.GetMatchingScore(request, requestMatchResult).Should().Be(1.0);
        }
    }
}
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests.RequestBuilders
{
    public class RequestBuilderWithBodyTests
    {
        [Fact]
        public void RequestBuilder_WithBody_IMatcher()
        {
            // Assign
            var matcher = new WildcardMatcher("x");

            // Act
            var requestBuilder = (Request)Request.Create().WithBody(matcher);

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            matchers.Should().HaveCount(1);
            ((RequestMessageBodyMatcher)matchers[0]).Matchers.Should().Contain(matcher);
        }

        [Fact]
        public void RequestBuilder_WithBody_IMatchers()
        {
            // Assign
            var matcher1 = new WildcardMatcher("x");
            var matcher2 = new WildcardMatcher("y");

            // Act
            var requestBuilder = (Request)Request.Create().WithBody(new[] { matcher1, matcher2 }.Cast<IMatcher>().ToArray());

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            matchers.Should().HaveCount(1);
            ((RequestMessageBodyMatcher)matchers[0]).Matchers.Should().Contain(new[] { matcher1, matcher2 });
        }
    }
}
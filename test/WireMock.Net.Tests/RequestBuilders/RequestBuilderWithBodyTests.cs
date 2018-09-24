using System.Collections.Generic;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests
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
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(((RequestMessageBodyMatcher) matchers[0]).Matcher).IsEqualTo(matcher);
        }
    }
}
using System.Collections.Generic;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests
{
    public class RequestBuilderWithParamTests
    {
        [Fact]
        public void RequestBuilder_WithParam_String_MatchBehaviour()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithParam("p", MatchBehaviour.AcceptOnMatch);

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
        }

        [Fact]
        public void RequestBuilder_WithParam_String_Strings()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithParam("p", new[] { "v1", "v2" });

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
        }

        [Fact]
        public void RequestBuilder_WithParam_String_IExactMatcher()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithParam("p", new ExactMatcher("v"));

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
        }

        [Fact]
        public void RequestBuilder_WithParam_String_MatchBehaviour_IExactMatcher()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithParam("p", MatchBehaviour.AcceptOnMatch, new ExactMatcher("v"));

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageParamMatcher));
        }
    }
}
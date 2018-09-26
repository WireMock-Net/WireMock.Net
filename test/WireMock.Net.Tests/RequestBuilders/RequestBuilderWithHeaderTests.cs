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
    public class RequestBuilderWithHeaderTests
    {
        [Fact]
        public void RequestBuilder_WithHeader_String_String_MatchBehaviour()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithHeader("h", "t", MatchBehaviour.AcceptOnMatch);

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
        }

        [Fact]
        public void RequestBuilder_WithHeader_String_String_Bool_MatchBehaviour()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithHeader("h", "t", true, MatchBehaviour.AcceptOnMatch);

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
        }

        [Fact]
        public void RequestBuilder_WithHeader_String_Strings_MatchBehaviour()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithHeader("h", new[] { "t1", "t2" }, MatchBehaviour.AcceptOnMatch);

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
        }

        [Fact]
        public void RequestBuilder_WithHeader_String_Strings_Bool_MatchBehaviour()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithHeader("h", new[] { "t1", "t2" }, true, MatchBehaviour.AcceptOnMatch);

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
        }

        [Fact]
        public void RequestBuilder_WithHeader_String_IStringMatcher()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithHeader("h", new ExactMatcher("v"));

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
        }

        [Fact]
        public void RequestBuilder_WithHeader_FuncIDictionary()
        {
            // Act
            var requestBuilder = (Request)Request.Create().WithHeader((IDictionary<string, string[]> x) => true);

            // Assert
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageHeaderMatcher));
        }
    }
}
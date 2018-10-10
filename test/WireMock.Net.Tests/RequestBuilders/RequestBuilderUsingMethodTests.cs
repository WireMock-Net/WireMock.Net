using System.Collections.Generic;
using NFluent;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using Xunit;

namespace WireMock.Net.Tests.RequestBuilders
{
    public class RequestBuilderUsingMethodTests
    {
        [Fact]
        public void RequestBuilder_UsingPatch()
        {
            // Act
            var requestBuilder = (Request)Request.Create().UsingPatch();

            // Assert 1
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That((matchers[0] as RequestMessageMethodMatcher).Methods).ContainsExactly("PATCH");
        }

        [Fact]
        public void RequestBuilder_UsingAnyMethod_ClearsAllOtherMatches()
        {
            // Assign
            var requestBuilder = (Request)Request.Create().UsingGet();

            // Assert 1
            var matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(1);
            Check.That(matchers[0]).IsInstanceOfType(typeof(RequestMessageMethodMatcher));

            // Act
            requestBuilder.UsingAnyMethod();

            // Assert 2
            matchers = requestBuilder.GetPrivateFieldValue<IList<IRequestMatcher>>("_requestMatchers");
            Check.That(matchers.Count()).IsEqualTo(0);
        }
    }
}
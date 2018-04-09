using System;
using System.Collections.Generic;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Xunit;

namespace WireMock.Net.Tests.RequestMatchers
{
    public class RequestMessageCookieMatcherTests
    {
        [Fact]
        public void RequestMessageCookieMatcher_GetMatchingScore_IStringMatcher_Match()
        {
            // Assign
            var cookies = new Dictionary<string, string> { { "cook", "x" } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
            var matcher = new RequestMessageCookieMatcher("cook", new ExactMatcher("x"));

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageCookieMatcher_GetMatchingScore_Func_Match()
        {
            // Assign
            var cookies = new Dictionary<string, string> { { "cook", "x" } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
            var matcher = new RequestMessageCookieMatcher(x => x.ContainsKey("cook"));

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }
    }
}
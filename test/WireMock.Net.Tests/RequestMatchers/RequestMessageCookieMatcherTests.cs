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
        public void RequestMessageCookieMatcher_GetMatchingScore_AcceptOnMatch_CookieDoesNotExists()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1");
            var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "c", "x");

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }

        [Fact]
        public void RequestMessageCookieMatcher_GetMatchingScore_RejectOnMatch_CookieDoesNotExists()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1");
            var matcher = new RequestMessageCookieMatcher(MatchBehaviour.RejectOnMatch, "c", "x");

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageCookieMatcher_GetMatchingScore_AcceptOnMatch_CookieDoesNotMatchPattern()
        {
            // Assign
            var cookies = new Dictionary<string, string> { { "c", "x" } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
            var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "no-match", "123");

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }
        [Fact]
        public void RequestMessageCookieMatcher_GetMatchingScore_RejectOnMatch_CookieDoesNotMatchPattern()
        {
            // Assign
            var cookies = new Dictionary<string, string> { { "h", "x" } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
            var matcher = new RequestMessageCookieMatcher(MatchBehaviour.RejectOnMatch, "no-match", "123");

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageCookieMatcher_GetMatchingScore_AcceptOnMatch()
        {
            // Assign
            var cookies = new Dictionary<string, string> { { "h", "x" } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
            var matcher = new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, "h", "x");

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageCookieMatcher_GetMatchingScore_RejectOnMatch()
        {
            // Assign
            var cookies = new Dictionary<string, string> { { "h", "x" } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, null, cookies);
            var matcher = new RequestMessageCookieMatcher(MatchBehaviour.RejectOnMatch, "h", "x");

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }

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
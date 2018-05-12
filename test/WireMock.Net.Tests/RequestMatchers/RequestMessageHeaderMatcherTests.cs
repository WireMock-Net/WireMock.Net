using System;
using System.Collections.Generic;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Xunit;

namespace WireMock.Net.Tests.RequestMatchers
{
    public class RequestMessageHeaderMatcherTests
    {
        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_AcceptOnMatch_HeaderDoesNotExists()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1");
            var matcher = new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "h", "x", true);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }

        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_RejectOnMatch_HeaderDoesNotExists()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1");
            var matcher = new RequestMessageHeaderMatcher(MatchBehaviour.RejectOnMatch, "h", "x", true);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_AcceptOnMatch_HeaderDoesNotMatchPattern()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, headers);
            var matcher = new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "no-match", "123", true);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }
        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_RejectOnMatch_HeaderDoesNotMatchPattern()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, headers);
            var matcher = new RequestMessageHeaderMatcher(MatchBehaviour.RejectOnMatch, "no-match", "123", true);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_AcceptOnMatch()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, headers);
            var matcher = new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "h", "x", true);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_RejectOnMatch()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, headers);
            var matcher = new RequestMessageHeaderMatcher(MatchBehaviour.RejectOnMatch, "h", "x", true);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }

        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_IStringMatcher_Match()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, headers);
            var matcher = new RequestMessageHeaderMatcher("h", new ExactMatcher("x"));

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageHeaderMatcher_GetMatchingScore_Func_Match()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "h", new[] { "x" } } };
            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", null, headers);
            var matcher = new RequestMessageHeaderMatcher(x => x.ContainsKey("h"));

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }
    }
}
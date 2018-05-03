using System;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Xunit;

namespace WireMock.Net.Tests.RequestMatchers
{
    public class RequestMessageParamMatcherTests
    {
        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_AllMatch()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost?key=test1,test2"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_PartialMatch()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost?key=test0,test2"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresent()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost?key"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresent_WithNull()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost?key"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key");

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresent_WithEmptyArray()
        {
            // Assign
            var requestMessage = new RequestMessage(new Uri("http://localhost?key"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", new string[] { });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }
    }
}
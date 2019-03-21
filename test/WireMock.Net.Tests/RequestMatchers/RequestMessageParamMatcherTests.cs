using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using Xunit;

namespace WireMock.Net.Tests.RequestMatchers
{
    public class RequestMessageParamMatcherTests
    {
        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_IgnoreCaseKeyWithValuesPresentInUrl_MatchExactOnStringValues()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "KeY", true, new[] { "test1" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_KeyWithValuesPresentInUrl_MatchExactOnStringValues_Returns0_5()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_KeyWith3ValuesPresentInUrl_MatchExactOn2StringValues_Returns0_5()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1,test2,test3"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsCloseTo(0.66d, 0.1d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_KeyWithValuesPresentInUrl_MatchExactOnStringValues_Returns1_0()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1,test2"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_KeyWithValuesPresentInUrl_MatchExactOnExactMatchers()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test1,test2"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new IStringMatcher[] { new ExactMatcher("test1"), new ExactMatcher("test2") });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_KeyWithValuesPresentInUrl_MatchOnKeyWithValues_PartialMatch()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=test0,test2"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresentInUrl_MatchOnKeyWithValues_Fails()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new[] { "test1", "test2" });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresentInUrl_MatchOnKey()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_OnlyKeyPresentInUrl_MatchOnKeyWithEmptyArray()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false, new string[] { });

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }

        [Fact]
        public void RequestMessageParamMatcher_GetMatchingScore_KeyWithValuePresentInUrl_MatchOnKey()
        {
            // Assign
            var requestMessage = new RequestMessage(new UrlDetails("http://localhost?key=frank@contoso.com"), "GET", "127.0.0.1");
            var matcher = new RequestMessageParamMatcher(MatchBehaviour.AcceptOnMatch, "key", false);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
        }
    }
}
using System.Linq;
using Moq;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.RequestMatchers
{
    public class RequestMessageBodyMatcherTests
    {
        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsString_IStringMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "b",
                DetectedBodyType = BodyType.String
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);

            // Verify
            stringMatcherMock.Verify(m => m.GetPatterns(), Times.Never);
            stringMatcherMock.Verify(m => m.IsMatch("b"), Times.Once);
        }

        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsString_IStringMatchers()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "b",
                DetectedBodyType = BodyType.String
            };
            var stringMatcherMock1 = new Mock<IStringMatcher>();
            stringMatcherMock1.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.2d);
            var stringMatcherMock2 = new Mock<IStringMatcher>();
            stringMatcherMock2.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.8d);
            var matchers = new[] { stringMatcherMock1.Object, stringMatcherMock2.Object };

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(matchers.Cast<IMatcher>().ToArray());

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.8d);

            // Verify
            stringMatcherMock1.Verify(m => m.GetPatterns(), Times.Never);
            stringMatcherMock1.Verify(m => m.IsMatch("b"), Times.Once);
            stringMatcherMock2.Verify(m => m.GetPatterns(), Times.Never);
            stringMatcherMock2.Verify(m => m.IsMatch("b"), Times.Once);
        }

        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsBytes_IStringMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsBytes = new byte[] { 1 },
                DetectedBodyType = BodyType.Bytes
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);

            // Verify
            stringMatcherMock.Verify(m => m.GetPatterns(), Times.Never);
            stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_IStringMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new { value = 42 },
                DetectedBodyType = BodyType.Json
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);

            // Verify
            stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_and_BodyAsString_IStringMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new { value = 42 },
                BodyAsString = "orig",
                DetectedBodyType = BodyType.Json
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);

            // Verify
            stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_IObjectMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = 42,
                DetectedBodyType = BodyType.Json
            };
            var objectMatcherMock = new Mock<IObjectMatcher>();
            objectMatcherMock.Setup(m => m.IsMatch(It.IsAny<object>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(objectMatcherMock.Object);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);

            // Verify
            objectMatcherMock.Verify(m => m.IsMatch(42), Times.Once);
        }

        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsBytes_IObjectMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsBytes = new byte[] { 1 },
                DetectedBodyType = BodyType.Bytes
            };
            var objectMatcherMock = new Mock<IObjectMatcher>();
            objectMatcherMock.Setup(m => m.IsMatch(It.IsAny<object>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(objectMatcherMock.Object);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.5d);

            // Verify
            objectMatcherMock.Verify(m => m.IsMatch(It.IsAny<byte[]>()), Times.Once);
        }
    }
}

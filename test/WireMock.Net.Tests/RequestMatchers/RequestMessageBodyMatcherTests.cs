using System;
using Moq;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
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
                BodyAsString = "b"
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", body);

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
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsBytes_IStringMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsBytes = new byte[] { 1 }
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", body);

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
                BodyAsJson = new { value = 42 }
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(stringMatcherMock.Object);

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(0.0d);

            // Verify
            stringMatcherMock.Verify(m => m.IsMatch(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_and_BodyAsString_IStringMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new { value = 42 },
                BodyAsString = "orig"
            };
            var stringMatcherMock = new Mock<IStringMatcher>();
            stringMatcherMock.Setup(m => m.IsMatch(It.IsAny<string>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", body);

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
                BodyAsJson = 42
            };
            var objectMatcherMock = new Mock<IObjectMatcher>();
            objectMatcherMock.Setup(m => m.IsMatch(It.IsAny<object>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", body);

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
                BodyAsBytes = new byte[] { 1 }
            };
            var objectMatcherMock = new Mock<IObjectMatcher>();
            objectMatcherMock.Setup(m => m.IsMatch(It.IsAny<object>())).Returns(0.5d);

            var requestMessage = new RequestMessage(new Uri("http://localhost"), "GET", "127.0.0.1", body);

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

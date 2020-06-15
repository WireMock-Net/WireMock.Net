﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NFluent;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Types;
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
        public void RequestMessageBodyMatcher_GetMatchingScore_BodyAsJson_CSharpCodeMatcher()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new { value = 42 },
                DetectedBodyType = BodyType.Json
            };

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", body);

            var matcher = new RequestMessageBodyMatcher(new CSharpCodeMatcher(MatchBehaviour.AcceptOnMatch, "return it.value == 42;"));

            // Act
            var result = new RequestMatchResult();
            double score = matcher.GetMatchingScore(requestMessage, result);

            // Assert
            Check.That(score).IsEqualTo(1.0d);
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

        [Theory]
        [MemberData(nameof(MatchingScoreData))]
        public async Task RequestMessageBodyMatcher_GetMatchingScore_Funcs_Matching(object body, RequestMessageBodyMatcher matcher, bool shouldMatch)
        {
            // assign
            BodyData bodyData;
            if (body is byte[] b)
            {
                var bodyParserSettings = new BodyParserSettings
                {
                    Stream = new MemoryStream(b),
                    ContentType = null,
                    DeserializeJson = true
                };
                bodyData = await BodyParser.Parse(bodyParserSettings);
            }
            else if (body is string s)
            {
                var bodyParserSettings = new BodyParserSettings
                {
                    Stream = new MemoryStream(Encoding.UTF8.GetBytes(s)),
                    ContentType = null,
                    DeserializeJson = true
                };
                bodyData = await BodyParser.Parse(bodyParserSettings);
            }
            else
            {
                throw new Exception();
            }

            var requestMessage = new RequestMessage(new UrlDetails("http://localhost"), "GET", "127.0.0.1", bodyData);

            // act
            var result = new RequestMatchResult();
            var score = matcher.GetMatchingScore(requestMessage, result);

            // assert
            Check.That(score).IsEqualTo(shouldMatch ? 1d : 0d);
        }

        public static TheoryData<object, RequestMessageBodyMatcher, bool> MatchingScoreData
        {
            get
            {
                var json = "{'a':'b'}";
                var str = "HelloWorld";
                var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00 };

                return new TheoryData<object, RequestMessageBodyMatcher, bool>
                {
                    // JSON match +++
                    {json, new RequestMessageBodyMatcher((object o) => ((dynamic) o).a == "b"), true},
                    {json, new RequestMessageBodyMatcher((string s) => s == json), true},
                    {json, new RequestMessageBodyMatcher((byte[] b) => b.SequenceEqual(Encoding.UTF8.GetBytes(json))), true},
                    
                    // JSON no match ---
                    {json, new RequestMessageBodyMatcher((object o) => false), false},
                    {json, new RequestMessageBodyMatcher((string s) => false), false},
                    {json, new RequestMessageBodyMatcher((byte[] b) => false), false},
                    {json, new RequestMessageBodyMatcher(), false },
                    
                    // string match +++
                    {str, new RequestMessageBodyMatcher((object o) => o == null), true},
                    {str, new RequestMessageBodyMatcher((string s) => s == str), true},
                    {str, new RequestMessageBodyMatcher((byte[] b) => b.SequenceEqual(Encoding.UTF8.GetBytes(str))), true},

                    // string no match ---
                    {str, new RequestMessageBodyMatcher((object o) => false), false},
                    {str, new RequestMessageBodyMatcher((string s) => false), false},
                    {str, new RequestMessageBodyMatcher((byte[] b) => false), false},
                    {str, new RequestMessageBodyMatcher(), false },
                    
                    // binary match +++
                    {bytes, new RequestMessageBodyMatcher((object o) => o == null), true},
                    {bytes, new RequestMessageBodyMatcher((string s) => s == null), true},
                    {bytes, new RequestMessageBodyMatcher((byte[] b) => b.SequenceEqual(bytes)), true},

                    // binary no match ---
                    {bytes, new RequestMessageBodyMatcher((object o) => false), false},
                    {bytes, new RequestMessageBodyMatcher((string s) => false), false},
                    {bytes, new RequestMessageBodyMatcher((byte[] b) => false), false},
                    {bytes, new RequestMessageBodyMatcher(), false },
                };
            }
        }
    }
}

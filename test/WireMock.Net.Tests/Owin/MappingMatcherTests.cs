using System;
using System.Collections.Concurrent;
using Moq;
using NFluent;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Owin;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Owin
{
    public class MappingMatcherTests
    {
        private readonly Mock<IWireMockMiddlewareOptions> _optionsMock;
        private readonly MappingMatcher _sut;

        public MappingMatcherTests()
        {
            _optionsMock = new Mock<IWireMockMiddlewareOptions>();
            _optionsMock.SetupAllProperties();
            _optionsMock.Setup(o => o.Mappings).Returns(new ConcurrentDictionary<Guid, IMapping>());
            _optionsMock.Setup(o => o.LogEntries).Returns(new ConcurrentObservableCollection<LogEntry>());
            _optionsMock.Setup(o => o.Scenarios).Returns(new ConcurrentDictionary<string, ScenarioState>());

            var loggerMock = new Mock<IWireMockLogger>();
            loggerMock.SetupAllProperties();
            loggerMock.Setup(l => l.Error(It.IsAny<string>()));
            _optionsMock.Setup(o => o.Logger).Returns(loggerMock.Object);

            _sut = new MappingMatcher(_optionsMock.Object);
        }

        [Fact]
        public void MappingMatcher_FindBestMatch_WhenNoMappingsDefined_ShouldReturnNull()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

            // Act
            var result = _sut.FindBestMatch(request);

            // Assert and Verify
            Check.That(result).IsNull();
        }

        [Fact]
        public void MappingMatcher_FindBestMatch_WhenMappingThrowsException_ShouldReturnNull()
        {
            // Assign
            var mappingMock = new Mock<IMapping>();
            mappingMock.Setup(m => m.GetRequestMatchResult(It.IsAny<RequestMessage>(), It.IsAny<string>())).Throws<Exception>();

            var mappings = new ConcurrentDictionary<Guid, IMapping>();
            mappings.TryAdd(Guid.NewGuid(), mappingMock.Object);

            _optionsMock.Setup(o => o.Mappings).Returns(mappings);

            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

            // Act
            var result = _sut.FindBestMatch(request);

            // Assert and Verify
            Check.That(result).IsNull();
        }

        [Fact]
        public void MappingMatcher_FindBestMatch_WhenAllowPartialMappingIsFalse_ShouldReturnExactMatch()
        {
            // Assign
            var mappings = InitMappings(new[] { (Guid.Parse("00000000-0000-0000-0000-000000000001"), 0.1), (Guid.Parse("00000000-0000-0000-0000-000000000002"), 1.0) });
            _optionsMock.Setup(o => o.Mappings).Returns(mappings);

            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

            // Act
            var result = _sut.FindBestMatch(request);

            // Assert and Verify
            Check.That(result.Mapping.Guid).IsEqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            Check.That(result.RequestMatchResult.AverageTotalScore).IsEqualTo(1.0);
        }

        [Fact]
        public void MappingMatcher_FindBestMatch_WhenAllowPartialMappingIsTrue_ShouldReturnAnyMatch()
        {
            // Assign
            _optionsMock.SetupGet(o => o.AllowPartialMapping).Returns(true);
            var mappings = InitMappings(new[] { (Guid.Parse("00000000-0000-0000-0000-000000000001"), 0.1), (Guid.Parse("00000000-0000-0000-0000-000000000002"), 0.9) });
            _optionsMock.Setup(o => o.Mappings).Returns(mappings);

            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");

            // Act
            var result = _sut.FindBestMatch(request);

            // Assert and Verify
            Check.That(result.Mapping.Guid).IsEqualTo(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            Check.That(result.RequestMatchResult.AverageTotalScore).IsEqualTo(0.9);
        }

        private ConcurrentDictionary<Guid, IMapping> InitMappings(params (Guid guid, double match)[] matches)
        {
            var mappings = new ConcurrentDictionary<Guid, IMapping>();

            foreach (var match in matches)
            {
                var mappingMock = new Mock<IMapping>();
                mappingMock.SetupGet(m => m.Guid).Returns(match.guid);

                var partialMatchResult = new RequestMatchResult();
                partialMatchResult.AddScore(typeof(object), match.match);
                mappingMock.Setup(m => m.GetRequestMatchResult(It.IsAny<RequestMessage>(), It.IsAny<string>())).Returns(partialMatchResult);

                mappings.TryAdd(match.guid, mappingMock.Object);
            }

            return mappings;
        }
    }
}
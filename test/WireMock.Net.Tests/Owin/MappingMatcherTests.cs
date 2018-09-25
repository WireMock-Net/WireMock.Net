using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private Mock<IWireMockMiddlewareOptions> _optionsMock;

        private IMappingMatcher _sut;

        public MappingMatcherTests()
        {
            _optionsMock = new Mock<IWireMockMiddlewareOptions>();
            _optionsMock.SetupAllProperties();
            _optionsMock.Setup(o => o.Mappings).Returns(new ConcurrentDictionary<Guid, IMapping>());
            _optionsMock.Setup(o => o.LogEntries).Returns(new ConcurentObservableCollection<LogEntry>());
            _optionsMock.Setup(o => o.Scenarios).Returns(new ConcurrentDictionary<string, ScenarioState>());

            _sut = new MappingMatcher(_optionsMock.Object);
        }

        [Fact]
        public void MappingMatcher_Match_NoMappingsDefined()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new { x = 1 }
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", body);

            // Act
            var result = _sut.Match(request);

            // Assert and Verify
            Check.That(result.Mapping).IsNull();
            Check.That(result.RequestMatchResult).IsNull();
        }

        [Fact]
        public void MappingMatcher_Match_AllowPartialMapping()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new { x = 1 }
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", body);

            _optionsMock.SetupGet(o => o.AllowPartialMapping).Returns(true);

            var mappingMock = new Mock<IMapping>();
            var partialMatchResult = new RequestMatchResult();
            partialMatchResult.AddScore(typeof(object), 0.1);

            mappingMock.Setup(m => m.GetRequestMatchResult(It.IsAny<RequestMessage>(), It.IsAny<string>())).Returns(partialMatchResult);

            var mappings = new ConcurrentDictionary<Guid, IMapping>();
            mappings.TryAdd(Guid.NewGuid(), mappingMock.Object);

            _optionsMock.Setup(o => o.Mappings).Returns(mappings);

            // Act
            var result = _sut.Match(request);

            // Assert and Verify
            Check.That(result.Mapping).IsNotNull();
            Check.That(result.RequestMatchResult.AverageTotalScore).IsEqualTo(0.1);
        }
    }
}

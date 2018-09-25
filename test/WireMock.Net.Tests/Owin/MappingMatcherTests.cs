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
        private Mock<IWireMockMiddlewareOptions> _options;

        private IMappingMatcher _sut;

        public MappingMatcherTests()
        {
            _options = new Mock<IWireMockMiddlewareOptions>();
            _options.SetupAllProperties();
            _options.Setup(o => o.Mappings).Returns(new ConcurrentDictionary<Guid, Mapping>());
            _options.Setup(o => o.LogEntries).Returns(new ConcurentObservableCollection<LogEntry>());
            _options.Setup(o => o.Scenarios).Returns(new ConcurrentDictionary<string, ScenarioState>());
            
            _sut = new MappingMatcher(_options.Object);
        }

        [Fact]
        public void MappingMatcher_Match_NoMappingsDefined()
        {
            // Assign
            var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/json" } } };
            var body = new BodyData
            {
                BodyAsJson = new { x = 1 }
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", body, headers);

            // Act
            var result = _sut.Match(request);

            // Assert and Verify
            Check.That(result.Mapping).IsNull();
            Check.That(result.RequestMatchResult).IsNull();
        }

        //[Fact]
        //public void MappingMatcher_Match_AllowPartialMapping()
        //{
        //    // Assign
        //    var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/json" } } };
        //    var body = new BodyData
        //    {
        //        BodyAsJson = new { x = 1 }
        //    };
        //    var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1", body, headers);

        //    _options.SetupGet(o => o.AllowPartialMapping).Returns(true);

        //    var mappingMock = new Mock<Mapping>();
        //    var partialMatchResult = new RequestMatchResult();
        //    partialMatchResult.AddScore(typeof(object), 0.1);

        //    mappingMock.Setup(m => m.GetRequestMatchResult(It.IsAny<RequestMessage>(), It.IsAny<string>())).Returns(partialMatchResult);

        //    var mappings = new ConcurrentDictionary<Guid, Mapping>();
        //    mappings.TryAdd(Guid.NewGuid(), mappingMock.Object);

        //    _options.Setup(o => o.Mappings).Returns(mappings);

        //    // Act
        //    var result = _sut.Match(request);

        //    // Assert and Verify
        //    Check.That(result.Mapping).IsNotNull();
        //    Check.That(result.RequestMatchResult.AverageTotalScore).IsEqualTo(0.1);
        //}
    }
}

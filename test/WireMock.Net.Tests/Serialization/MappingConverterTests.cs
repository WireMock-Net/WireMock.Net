using System;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class MappingConverterTests
    {
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        private readonly MappingConverter _sut;

        public MappingConverterTests()
        {
            _sut = new MappingConverter(new MatcherMapper(_settings));
        }

        [Fact]
        public void ToMappingModel()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create();
            var mapping = new Mapping(Guid.NewGuid(), "", null, _settings, request, response, 0, null, null, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Priority.Should().BeNull();
            model.Response.BodyAsJsonIndented.Should().BeNull();
            model.Response.UseTransformer.Should().BeNull();
            model.Response.Headers.Should().BeNull();
        }

        [Fact]
        public void ToMappingModel_WithPriority_ReturnsPriority()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create().WithBodyAsJson(new { x = "x" }).WithTransformer();
            var mapping = new Mapping(Guid.NewGuid(), "", null, _settings, request, response, 42, null, null, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Priority.Should().Be(42);
            model.Response.UseTransformer.Should().BeTrue();
        }
    }
}
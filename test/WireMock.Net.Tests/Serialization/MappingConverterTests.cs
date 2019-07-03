using FluentAssertions;
using Moq;
using System;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class MappingConverterTests
    {
        private readonly Mock<IFluentMockServerSettings> _settingsMock = new Mock<IFluentMockServerSettings>();

        [Fact]
        public void ToMappingModel()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create();
            var mapping = new Mapping(Guid.NewGuid(), "", null, _settingsMock.Object, request, response, 0, null, null, null);

            // Act
            var model = MappingConverter.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Priority.Should().BeNull();
            model.Response.BodyAsJsonIndented.Should().BeNull();
            model.Response.UseTransformer.Should().BeNull();
        }

        [Fact]
        public void ToMappingModel_WithPriority_ReturnsPriority()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create().WithBodyAsJson(new { x = "x" }).WithTransformer();
            var mapping = new Mapping(Guid.NewGuid(), "", null, _settingsMock.Object, request, response, 42, null, null, null);

            // Act
            var model = MappingConverter.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Priority.Should().Be(42);
            model.Response.UseTransformer.Should().BeTrue();
        }
    }
}
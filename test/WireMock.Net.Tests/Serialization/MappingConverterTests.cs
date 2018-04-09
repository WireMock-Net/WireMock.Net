using System;
using NFluent;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class MappingConverterTests
    {
        [Fact]
        public void MappingConverter_ToMappingModel()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create();
            var mapping = new Mapping(Guid.NewGuid(), "", null, request, response, 0, null, null, null);

            // Act
            var model = MappingConverter.ToMappingModel(mapping);

            // Assert
            Check.That(model).IsNotNull();
        }
    }
}
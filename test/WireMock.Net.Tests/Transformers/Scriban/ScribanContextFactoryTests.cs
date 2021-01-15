using FluentAssertions;
using HandlebarsDotNet;
using Moq;
using System;
using WireMock.Handlers;
using WireMock.Transformers.Handlebars;
using WireMock.Transformers.Scriban;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests.Transformers.Handlebars
{
    public class ScribanContextFactoryTests
    {
        private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock = new Mock<IFileSystemHandler>();

        [Theory]
        [InlineData(TransformerType.Scriban)]
        [InlineData(TransformerType.ScribanDotLiquid)]
        public void Create_With_Scriban_TransformerType_Creates_ITransformerContext(TransformerType transformerType)
        {
            // Arrange
            var sut = new ScribanContextFactory(_fileSystemHandlerMock.Object, transformerType);

            // Act
            var result = sut.Create();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Create_With_Invalid_TransformerType_Throws_Exception()
        {
            // Act
            Action action = () => new ScribanContextFactory(_fileSystemHandlerMock.Object, TransformerType.Handlebars);

            // Assert
            action.Should().Throw<Exception>();
        }
    }
}
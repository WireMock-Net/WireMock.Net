using System;
using FluentAssertions;
using HandlebarsDotNet;
using Moq;
using WireMock.Handlers;
using WireMock.Transformers.Handlebars;
using Xunit;

namespace WireMock.Net.Tests.Transformers.Handlebars
{
    public class HandlebarsContextFactoryTests
    {
        private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock = new Mock<IFileSystemHandler>();

        [Fact]
        public void Create_WithNullAction_DoesNotInvokeAction()
        {
            // Arrange
            var sut = new HandlebarsContextFactory(_fileSystemHandlerMock.Object, null);

            // Act
            var result = sut.Create();

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithAction_InvokesAction()
        {
            // Arrange
            int num = 0;
            Action<IHandlebars, IFileSystemHandler> action = (ctx, fs) =>
            {
                ctx.Should().NotBeNull();
                fs.Should().NotBeNull();

                num++;
            };
            var sut = new HandlebarsContextFactory(_fileSystemHandlerMock.Object, action);

            // Act
            var result = sut.Create();

            // Assert
            result.Should().NotBeNull();

            // Verify
            num.Should().Be(1);
        }
    }
}

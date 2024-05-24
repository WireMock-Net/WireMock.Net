#if NET8_0_OR_GREATER
using System;
using Aspire.Hosting;
using FluentAssertions;
using Moq;
using Xunit;

namespace WireMock.Net.Tests.Aspire;

public class WireMockServerBuilderExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void AddWireMock_WithInvalidName_ShouldThrowException(string name)
    {
        // Arrange
        var builder = Mock.Of<IDistributedApplicationBuilder>();

        // Act
        Action act = () => builder.AddWireMock(name, 12345);

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void AddWireMock_WithInvalidPort_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int invalidPort = -1;
        var builder = Mock.Of<IDistributedApplicationBuilder>();

        // Act
        Action act = () => builder.AddWireMock("ValidName", invalidPort);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Specified argument was out of the range of valid values. (Parameter 'port')");
    }
}
#endif
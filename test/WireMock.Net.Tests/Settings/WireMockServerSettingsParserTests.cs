// Copyright © WireMock.Net

using FluentAssertions;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Settings;

public class WireMockServerSettingsParserTests
{
    [Fact]
    public void TryParseArguments_With_Args()
    {
        // Act
        var result = WireMockServerSettingsParser.TryParseArguments(new[]
        {
            "--adminPath", "ap"
        }, null, out var settings);

        // Assert
        result.Should().BeTrue();
        settings.Should().NotBeNull();
        settings!.AdminPath.Should().Be("ap");
    }

    [Fact]
    public void TryParseArguments_Without_Args()
    {
        // Act
        var result = WireMockServerSettingsParser.TryParseArguments(new string[] { }, null, out var settings);

        // Assert
        result.Should().BeTrue();
        settings.Should().NotBeNull();
        settings!.AdminPath.Should().Be("/__admin");
    }
}
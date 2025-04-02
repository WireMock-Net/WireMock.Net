// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using FluentAssertions;
using WireMock.Net.OpenApiParser.Utils;
using Xunit;

namespace WireMock.Net.Tests.OpenApiParser;

public class PathUtilsTests
{
    [Theory]
    [InlineData(new string[] { }, "")]
    [InlineData(new[] { "path1" }, "path1")]
    [InlineData(new[] { "/path1" }, "/path1")]
    [InlineData(new[] { "/path1/" }, "/path1")]
    public void Combine_ShouldReturnCombinedPathTest1(string[] paths, string expected)
    {
        // Act
        var result = PathUtils.Combine(paths);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("/path1", "path2")]
    [InlineData("/path1/", "path2")]
    [InlineData("/path1", "/path2")]
    [InlineData("/path1", "path2/")]
    public void Combine_ShouldReturnCombinedPathTest2(params string[] paths)
    {
        // Act
        var result = PathUtils.Combine(paths);

        // Assert
        result.Should().Be("/path1/path2");
    }
}
#endif
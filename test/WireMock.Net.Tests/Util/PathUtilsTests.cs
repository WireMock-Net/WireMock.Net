// Copyright © WireMock.Net

using NFluent;
using System.IO;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class PathUtilsTests
{
    [Theory]
    [InlineData(@"subdirectory/MyXmlResponse.xml")]
    [InlineData(@"subdirectory\MyXmlResponse.xml")]
    public void PathUtils_CleanPath(string path)
    {
        // Act
        var cleanPath = PathUtils.CleanPath(path);

        // Assert
        Check.That(cleanPath).Equals("subdirectory" + Path.DirectorySeparatorChar + "MyXmlResponse.xml");
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData(@"/", "")]
    [InlineData(@"//", "")]
    [InlineData(@"//a", "a")]
    [InlineData(@"\", "")]
    [InlineData(@"\\", "")]
    [InlineData(@"\\a", "a")]
    public void PathUtils_CleanPath_RemoveLeadingDirectorySeparators(string path, string expected)
    {
        // Arrange
        var cleanPath = PathUtils.CleanPath(path);

        // Act
        var withoutDirectorySeparators = PathUtils.RemoveLeadingDirectorySeparators(cleanPath);

        // Assert
        Check.That(withoutDirectorySeparators).Equals(expected);
    }
}
using System;
using Microsoft.Owin;
using NFluent;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util
{
    public class UriUtilsTests
    {
        [Fact]
        public void UriUtils_CreateUri_WithValidPathString()
        {
            // Assign
            Uri uri = new Uri("https://localhost:1234/a/b?x=0");

            // Act
            Uri result = UriUtils.CreateUri(uri, new PathString("/a"));

            // Assert
            Check.That(result.ToString()).Equals("https://localhost:1234/b?x=0");
        }

        [Fact]
        public void UriUtils_CreateUri_WithEmptyPathString()
        {
            // Assign
            Uri uri = new Uri("https://localhost:1234/a/b?x=0");

            // Act
            Uri result = UriUtils.CreateUri(uri, new PathString());

            // Assert
            Check.That(result.ToString()).Equals("https://localhost:1234/a/b?x=0");
        }

        [Fact]
        public void UriUtils_CreateUri_WithDifferentPathString()
        {
            // Assign
            Uri uri = new Uri("https://localhost:1234/a/b?x=0");

            // Act
            Uri result = UriUtils.CreateUri(uri, new PathString("/test"));

            // Assert
            Check.That(result.ToString()).Equals("https://localhost:1234/a/b?x=0");
        }
    }
}

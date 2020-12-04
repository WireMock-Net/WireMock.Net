using System;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Plugin;
using Xunit;

namespace WireMock.Net.Tests.Plugin
{

    public class PluginLoaderTests
    {
        public interface IDummy
        {
        }

        [Fact]
        public void Load_Valid()
        {
            // Act
            var result = PluginLoader.Load<ICSharpCodeMatcher>(MatchBehaviour.AcceptOnMatch, "x");

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void Load_Invalid_ThrowsException()
        {
            // Act
            Action a = () => PluginLoader.Load<IDummy>();

            // Assert
            a.Should().Throw<DllNotFoundException>();
        }
    }
}

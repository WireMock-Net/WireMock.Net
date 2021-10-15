using System;
using AnyOfTypes;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Models;
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
            AnyOf<string, StringPattern> pattern = "x";
            var result = PluginLoader.Load<ICSharpCodeMatcher>(MatchBehaviour.AcceptOnMatch, pattern);

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
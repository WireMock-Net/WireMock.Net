using FluentAssertions;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class NotNullOrEmptyMatcherTests
    {
        [Fact]
        public void NotNullOrEmptyMatcher_GetName()
        {
            // Act
            var matcher = new NotNullOrEmptyMatcher();
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("NotNullOrEmptyMatcher");
        }

        [Theory]
        [InlineData(null, 0.0)]
        [InlineData(new byte[0], 0.0)]
        [InlineData(new byte[] { 48 }, 1.0)]
        public void NotNullOrEmptyMatcher_IsMatch_ByteArray(byte[] data, double expected)
        {
            // Act
            var matcher = new NotNullOrEmptyMatcher();
            double result = matcher.IsMatch(data);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, 0.0)]
        [InlineData("", 0.0)]
        [InlineData("x", 1.0)]
        public void NotNullOrEmptyMatcher_IsMatch_String(string @string, double expected)
        {
            // Act
            var matcher = new NotNullOrEmptyMatcher();
            double result = matcher.IsMatch(@string);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, 0.0)]
        [InlineData("", 0.0)]
        [InlineData("x", 1.0)]
        public void NotNullOrEmptyMatcher_IsMatch_StringAsObject(string @string, double expected)
        {
            // Act
            var matcher = new NotNullOrEmptyMatcher();
            double result = matcher.IsMatch((object)@string);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void NotNullOrEmptyMatcher_IsMatch_Json()
        {
            // Act
            var matcher = new NotNullOrEmptyMatcher();
            double result = matcher.IsMatch(new { x = "x" });

            // Assert
            result.Should().Be(1.0);
        }
    }
}
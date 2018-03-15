using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class JsonPathMatcherTests
    {
        [Fact]
        public void JsonPathMatcher_GetName()
        {
            // Assign
            var matcher = new JsonPathMatcher("X");

            // Act
            string name = matcher.GetName();

            // Assert
            Check.That(name).Equals("JsonPathMatcher");
        }

        [Fact]
        public void JsonPathMatcher_GetPatterns()
        {
            // Assign
            var matcher = new JsonPathMatcher("X");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("X");
        }
    }
}
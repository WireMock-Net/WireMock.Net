using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class XPathMatcherTests
    {
        [Fact]
        public void XPathMatcher_GetName()
        {
            // Assign
            var matcher = new XPathMatcher("X");

            // Act
            string name = matcher.GetName();

            // Assert
            Check.That(name).Equals("XPathMatcher");
        }

        [Fact]
        public void XPathMatcher_GetPatterns()
        {
            // Assign
            var matcher = new XPathMatcher("X");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("X");
        }
    }
}
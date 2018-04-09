using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class ExactMatcherTests
    {
        [Fact]
        public void ExactMatcher_GetName()
        {
            // Assign
            var matcher = new ExactMatcher("X");

            // Act
            string name = matcher.GetName();

            // Assert
            Check.That(name).Equals("ExactMatcher");
        }

        [Fact]
        public void ExactMatcher_GetPatterns()
        {
            // Assign
            var matcher = new ExactMatcher("X");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("X");
        }

        [Fact]
        public void ExactMatcher_IsMatch_MultiplePatterns()
        {
            // Assign
            var matcher = new ExactMatcher("x", "y");

            // Act
            double result = matcher.IsMatch("x");

            // Assert
            Check.That(result).IsEqualTo(0.5d);
        }

        [Fact]
        public void Request_WithBodyExactMatcher_false()
        {
            // Assign
            var matcher = new ExactMatcher("cat");

            // Act
            double result = matcher.IsMatch("caR");

            // Assert
            Check.That(result).IsStrictlyLessThan(1.0);
        }
    }
}
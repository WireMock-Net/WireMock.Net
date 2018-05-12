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
            string name = matcher.Name;

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
        public void ExactMatcher_IsMatch_SinglePattern()
        {
            // Assign
            var matcher = new ExactMatcher("cat");

            // Act
            double result = matcher.IsMatch("caR");

            // Assert
            Check.That(result).IsEqualTo(0.0);
        }

        [Fact]
        public void ExactMatcher_IsMatch_SinglePattern_AcceptOnMatch()
        {
            // Assign
            var matcher = new ExactMatcher(MatchBehaviour.AcceptOnMatch, "cat");

            // Act
            double result = matcher.IsMatch("cat");

            // Assert
            Check.That(result).IsEqualTo(1.0);
        }

        [Fact]
        public void ExactMatcher_IsMatch_SinglePattern_RejectOnMatch()
        {
            // Assign
            var matcher = new ExactMatcher(MatchBehaviour.RejectOnMatch, "cat");

            // Act
            double result = matcher.IsMatch("cat");

            // Assert
            Check.That(result).IsEqualTo(0.0);
        }
    }
}
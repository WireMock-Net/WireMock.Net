using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class LinqMatcherTests
    {
        [Fact]
        public void LinqMatcher_SinglePattern_IsMatch_Positive()
        {
            // Assign
            string input = "2018-08-31 13:59:59";

            // Act
            var matcher = new LinqMatcher("DateTime.Parse(it) > \"2018-08-01 13:50:00\"");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(1.0d);
        }

        [Fact]
        public void LinqMatcher_IsMatch_Negative()
        {
            // Assign
            string input = "2018-08-31 13:59:59";

            // Act
            var matcher = new LinqMatcher("DateTime.Parse(it) > \"2019-01-01 00:00:00\"");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
        }

        [Fact]
        public void LinqMatcher_IsMatch_RejectOnMatch()
        {
            // Assign
            string input = "2018-08-31 13:59:59";

            // Act
            var matcher = new LinqMatcher(MatchBehaviour.RejectOnMatch, "DateTime.Parse(it) > \"2018-08-01 13:50:00\"");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
        }

        [Fact]
        public void LinqMatcher_GetName()
        {
            // Assign
            var matcher = new LinqMatcher("x");

            // Act
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("LinqMatcher");
        }

        [Fact]
        public void LinqMatcher_GetPatterns()
        {
            // Assign
            var matcher = new LinqMatcher("x");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("x");
        }
    }
}
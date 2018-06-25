using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class ExactObjectMatcherTests
    {
        [Fact]
        public void ExactObjectMatcher_GetName()
        {
            // Assign
            object obj = 1;

            // Act
            var matcher = new ExactObjectMatcher(obj);
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("ExactObjectMatcher");
        }

        [Fact]
        public void ExactObjectMatcher_IsMatch_ByteArray()
        {
            // Assign
            object checkValue = new byte[] { 1, 2 };

            // Act
            var matcher = new ExactObjectMatcher(new byte[] { 1, 2 });
            double result = matcher.IsMatch(checkValue);

            // Assert
            Check.That(result).IsEqualTo(1.0);
        }

        [Fact]
        public void ExactObjectMatcher_IsMatch_AcceptOnMatch()
        {
            // Assign
            object obj = new { x = 500, s = "s" };

            // Act
            var matcher = new ExactObjectMatcher(obj);
            double result = matcher.IsMatch(new { x = 500, s = "s" });

            // Assert
            Check.That(result).IsEqualTo(1.0);
        }

        [Fact]
        public void ExactObjectMatcher_IsMatch_RejectOnMatch()
        {
            // Assign
            object obj = new { x = 500, s = "s" };

            // Act
            var matcher = new ExactObjectMatcher(MatchBehaviour.RejectOnMatch, obj);
            double result = matcher.IsMatch(new { x = 500, s = "s" });

            // Assert
            Check.That(result).IsEqualTo(0.0);
        }
    }
}
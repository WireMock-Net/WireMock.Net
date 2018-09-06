using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class LinqMatcherTests
    {
        [Fact]
        public void LinqMatcher_For_String_SinglePattern_IsMatch_Positive()
        {
            // Assign
            string input = "2018-08-31 13:59:59";

            // Act
            var matcher = new LinqMatcher("DateTime.Parse(it) > \"2018-08-01 13:50:00\"");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(1.0d);
        }

        [Fact]
        public void LinqMatcher_For_String_IsMatch_Negative()
        {
            // Assign
            string input = "2018-08-31 13:59:59";

            // Act
            var matcher = new LinqMatcher("DateTime.Parse(it) > \"2019-01-01 00:00:00\"");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
        }

        [Fact]
        public void LinqMatcher_For_String_IsMatch_RejectOnMatch()
        {
            // Assign
            string input = "2018-08-31 13:59:59";

            // Act
            var matcher = new LinqMatcher(MatchBehaviour.RejectOnMatch, "DateTime.Parse(it) > \"2018-08-01 13:50:00\"");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
        }

        [Fact]
        public void LinqMatcher_For_Object_IsMatch()
        {
            // Assign
            var input = new
            {
                Id = 9,
                Name = "Test"
            };

            // Act
            var matcher = new LinqMatcher("Id > 1 AND Name == \"Test\"");
            double match = matcher.IsMatch(input);

            // Assert
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void LinqMatcher_For_JObject_IsMatch()
        {
            // Assign
            var input = new JObject
            {
                { "Id", new JValue(9) },
                { "Name", new JValue("Test") }
            };

            // Act
            var matcher = new LinqMatcher("Id > 1 AND Name == \"Test\"");
            double match = matcher.IsMatch(input);

            // Assert
            Assert.Equal(1.0, match);
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
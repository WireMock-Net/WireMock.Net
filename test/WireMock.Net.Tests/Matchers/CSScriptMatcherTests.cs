#if !NET452
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class CSScriptMatcherTests
    {
        [Fact]
        public void CSScriptMatcher_For_String_SinglePattern_IsMatch_Positive()
        {
            // Assign
            string input = "x";

            // Act
            var matcher = new CSScriptMatcher("return it == \"x\";");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(1.0d);
        }

        [Fact]
        public void CSScriptMatcher_For_String_IsMatch_Negative()
        {
            // Assign
            string input = "y";

            // Act
            var matcher = new CSScriptMatcher("return it == \"x\";");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
        }

        [Fact]
        public void CSScriptMatcher_For_String_IsMatch_RejectOnMatch()
        {
            // Assign
            string input = "x";

            // Act
            var matcher = new CSScriptMatcher(MatchBehaviour.RejectOnMatch, "return it == \"x\";");

            // Assert
            Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
        }

        //[Fact]
        //public void CSScriptMatcher_For_Object_IsMatch()
        //{
        //    // Assign
        //    var input = new
        //    {
        //        Id = 9,
        //        Name = "Test"
        //    };

        //    // Act
        //    var matcher = new CSScriptMatcher("Id > 1 AND Name == \"Test\"");
        //    double match = matcher.IsMatch(input);

        //    // Assert
        //    Assert.Equal(1.0, match);
        //}

        //[Fact]
        //public void CSScriptMatcher_For_JObject_IsMatch()
        //{
        //    // Assign
        //    var input = new JObject
        //    {
        //        { "Id", new JValue(9) },
        //        { "Name", new JValue("Test") }
        //    };

        //    // Act
        //    var matcher = new CSScriptMatcher("Id > 1 AND Name == \"Test\"");
        //    double match = matcher.IsMatch(input);

        //    // Assert
        //    Assert.Equal(1.0, match);
        //}

        [Fact]
        public void CSScriptMatcher_GetName()
        {
            // Assign
            var matcher = new CSScriptMatcher("x");

            // Act
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("CSScriptMatcher");
        }

        [Fact]
        public void CSScriptMatcher_GetPatterns()
        {
            // Assign
            var matcher = new CSScriptMatcher("x");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("x");
        }
    }
}
#endif
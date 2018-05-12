using Newtonsoft.Json.Linq;
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
            string name = matcher.Name;

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

        [Fact]
        public void JsonPathMatcher_IsMatch_NullString()
        {
            // Assign
            string s = null;
            var matcher = new JsonPathMatcher("");

            // Act 
            double match = matcher.IsMatch(s);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPathMatcher_IsMatch_NullObject()
        {
            // Assign
            object o = null;
            var matcher = new JsonPathMatcher("");

            // Act 
            double match = matcher.IsMatch(o);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPathMatcher_IsMatch_String_Exception_Mismatch()
        {
            // Assign
            var matcher = new JsonPathMatcher("xxx");

            // Act 
            double match = matcher.IsMatch("");

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPathMatcher_IsMatch_Object_Exception_Mismatch()
        {
            // Assign
            var matcher = new JsonPathMatcher("");

            // Act 
            double match = matcher.IsMatch("x");

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPathMatcher_IsMatch_AnonymousObject()
        {
            // Assign 
            var matcher = new JsonPathMatcher("$..[?(@.Id == 1)]");

            // Act 
            double match = matcher.IsMatch(new { Id = 1, Name = "Test" });

            // Assert 
            Check.That(match).IsEqualTo(1);
        }

        [Fact]
        public void JsonPathMatcher_IsMatch_JObject()
        {
            // Assign 
            string[] patterns = { "$..[?(@.Id == 1)]" };
            var matcher = new JsonPathMatcher(patterns);

            // Act 
            var jobject = new JObject
            {
                { "Id", new JValue(1) },
                { "Name", new JValue("Test") }
            };
            double match = matcher.IsMatch(jobject);

            // Assert 
            Check.That(match).IsEqualTo(1);
        }

        [Fact]
        public void JsonPathMatcher_IsMatch_JObject_Parsed()
        {
            // Assign 
            var matcher = new JsonPathMatcher("$..[?(@.Id == 1)]");

            // Act 
            double match = matcher.IsMatch(JObject.Parse("{\"Id\":1,\"Name\":\"Test\"}"));

            // Assert 
            Check.That(match).IsEqualTo(1);
        }

        [Fact]
        public void JsonPathMatcher_IsMatch_RejectOnMatch()
        {
            // Assign
            var matcher = new JsonPathMatcher(MatchBehaviour.RejectOnMatch, "$..[?(@.Id == 1)]");

            // Act
            double match = matcher.IsMatch(JObject.Parse("{\"Id\":1,\"Name\":\"Test\"}"));

            // Assert
            Check.That(match).IsEqualTo(0.0);
        }
    }
}
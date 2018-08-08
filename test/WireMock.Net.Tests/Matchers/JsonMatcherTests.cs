using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class JsonMatcherTests
    {
        [Fact]
        public void JsonMatcher_GetName()
        {
            // Assign
            var matcher = new JsonMatcher("{}");

            // Act
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("JsonMatcher");
        }

        [Fact]
        public void JsonMatcher_GetValue()
        {
            // Assign
            var matcher = new JsonMatcher("{}");

            // Act
            object value = matcher.Value;

            // Assert
            Check.That(value).Equals("{}");
        }

        [Fact]
        public void JsonMatcher_IsMatch_ByteArray()
        {
            // Assign
            var bytes = new byte[0];
            var matcher = new JsonMatcher("");

            // Act 
            double match = matcher.IsMatch(bytes);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonMatcher_IsMatch_NullString()
        {
            // Assign
            string s = null;
            var matcher = new JsonMatcher("");

            // Act 
            double match = matcher.IsMatch(s);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonMatcher_IsMatch_NullObject()
        {
            // Assign
            object o = null;
            var matcher = new JsonMatcher("");

            // Act 
            double match = matcher.IsMatch(o);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonMatcher_IsMatch_JObject1()
        {
            // Assign 
            var matcher = new JsonMatcher(new { Id = 1, Name = "Test" });

            // Act 
            var jobject = new JObject
            {
                { "Id", new JValue(1) },
                { "Name", new JValue("Test") }
            };
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonMatcher_IsMatch_JObject2()
        {
            // Assign 
            var matcher = new JsonMatcher(new { Id = 1, Name = "Test" });

            // Act 
            var jobject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonMatcher_IsMatch_JObjectAsString()
        {
            // Assign 
            var matcher = new JsonMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

            // Act 
            var jobject = new JObject
            {
                { "Id", new JValue(1) },
                { "Name", new JValue("Test") }
            };
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonMatcher_IsMatch_JObjectAsString_RejectOnMatch()
        {
            // Assign 
            var matcher = new JsonMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

            // Act 
            var jobject = new JObject
            {
                { "Id", new JValue(1) },
                { "Name", new JValue("Test") }
            };
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(0.0, match);
        }
    }
}
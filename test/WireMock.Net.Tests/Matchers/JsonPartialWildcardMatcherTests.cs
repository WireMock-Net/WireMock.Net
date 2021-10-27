using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class JsonPartialWildcardMatcherTests
    {
        [Fact]
        public void JsonPartialWildcardMatcher_GetName()
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher("{}");

            // Act
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("JsonPartialWildcardMatcher");
        }

        [Fact]
        public void JsonPartialWildcardMatcher_GetValue()
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher("{}");

            // Act
            object value = matcher.Value;

            // Assert
            Check.That(value).Equals("{}");
        }

        [Fact]
        public void JsonPartialWildcardMatcher_WithInvalidStringValue_Should_ThrowException()
        {
            // Act
            Action action = () => new JsonPartialWildcardMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

            // Assert
            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void JsonPartialWildcardMatcher_WithInvalidObjectValue_Should_ThrowException()
        {
            // Act
            Action action = () => new JsonPartialWildcardMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

            // Assert
            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_WithInvalidValue_And_ThrowExceptionIsFalse_Should_ReturnMismatch()
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher("");

            // Act
            double match = matcher.IsMatch(new MemoryStream());

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_WithInvalidValue_And_ThrowExceptionIsTrue_Should_ReturnMismatch()
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher("", false, true);

            // Act
            Action action = () => matcher.IsMatch(new MemoryStream());

            // Assert 
            action.Should().Throw<JsonException>();
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_ByteArray()
        {
            // Assign
            var bytes = new byte[0];
            var matcher = new JsonPartialWildcardMatcher("");

            // Act 
            double match = matcher.IsMatch(bytes);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_NullString()
        {
            // Assign
            string s = null;
            var matcher = new JsonPartialWildcardMatcher("");

            // Act 
            double match = matcher.IsMatch(s);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_NullObject()
        {
            // Assign
            object o = null;
            var matcher = new JsonPartialWildcardMatcher("");

            // Act 
            double match = matcher.IsMatch(o);

            // Assert 
            Check.That(match).IsEqualTo(0);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_JArray()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher(new[] { "x", "y" });

            // Act 
            var jArray = new JArray
            {
                "x",
                "y"
            };
            double match = matcher.IsMatch(jArray);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_JObject()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher(new { Id = 1, Name = "Test" });

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
        public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JObject()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher(new { id = 1, Name = "test" }, true);

            // Act 
            var jobject = new JObject
            {
                { "Id", new JValue(1) },
                { "NaMe", new JValue("Test") }
            };
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_JObjectParsed()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher(new { Id = 1, Name = "Test" });

            // Act 
            var jobject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JObjectParsed()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher(new { Id = 1, Name = "TESt" }, true);

            // Act 
            var jobject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_JArrayAsString()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher("[ \"x\", \"y\" ]");

            // Act 
            var jArray = new JArray
            {
                "x",
                "y"
            };
            double match = matcher.IsMatch(jArray);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_JObjectAsString()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

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
        public void JsonPartialWildcardMatcher_IsMatch_WithIgnoreCaseTrue_JObjectAsString()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

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
        public void JsonPartialWildcardMatcher_IsMatch_JObjectAsString_RejectOnMatch()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

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

        [Fact]
        public void JsonPartialWildcardMatcher_IsMatch_JObjectWithDateTimeOffsetAsString()
        {
            // Assign 
            var matcher = new JsonPartialWildcardMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

            // Act 
            var jobject = new JObject
            {
                { "preferredAt", new JValue("2019-11-21T10:32:53.2210009+00:00") }
            };
            double match = matcher.IsMatch(jobject);

            // Assert 
            Assert.Equal(1.0, match);
        }

        [Theory]
        [InlineData("{\"test\":\"abc\"}", "{\"test\":\"abc\",\"other\":\"xyz\"}")]
        [InlineData("\"test\"", "\"test\"")]
        [InlineData("123", "123")]
        [InlineData("[\"test\"]", "[\"test\"]")]
        [InlineData("[\"test\"]", "[\"test\", \"other\"]")]
        [InlineData("[123]", "[123]")]
        [InlineData("[123]", "[123, 456]")]
        [InlineData("{ \"test\":\"value\" }", "{\"test\":\"value\",\"other\":123}")]
        [InlineData("{ \"test\":\"value\" }", "{\"test\":\"value\"}")]
        [InlineData("{\"test\":{\"nested\":\"value\"}}", "{\"test\":{\"nested\":\"value\"}}")]
        public void JsonPartialWildcardMatcher_IsMatch_StringInput_IsValidMatch(string value, string input)
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher(value);

            // Act
            double match = matcher.IsMatch(input);

            // Assert
            Assert.Equal(1.0, match);
        }

        [Theory]
        [InlineData("{\"test\":\"*\"}", "{\"test\":\"xxx\",\"other\":\"xyz\"}")]
        [InlineData("\"t*t\"", "\"test\"")]
        public void JsonPartialWildcardMatcher_IsMatch_StringInputWithWildcard_IsValidMatch(string value, string input)
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher(value);

            // Act
            double match = matcher.IsMatch(input);

            // Assert
            match.Should().Be(1.0);
        }

        [Theory]
        [InlineData("\"test\"", null)]
        [InlineData("\"test1\"", "\"test2\"")]
        [InlineData("123", "1234")]
        [InlineData("[\"test\"]", "[\"test1\"]")]
        [InlineData("[\"test\"]", "[\"test1\", \"test2\"]")]
        [InlineData("[123]", "[1234]")]
        [InlineData("{}", "\"test\"")]
        [InlineData("{ \"test\":\"value\" }", "{\"test\":\"value2\"}")]
        [InlineData("{ \"test.nested\":\"value\" }", "{\"test\":{\"nested\":\"value1\"}}")]
        [InlineData("{\"test\":{\"test1\":\"value\"}}", "{\"test\":{\"test1\":\"value1\"}}")]
        [InlineData("[{ \"test.nested\":\"value\" }]", "[{\"test\":{\"nested\":\"value1\"}}]")]
        public void JsonPartialWildcardMatcher_IsMatch_StringInputWithInvalidMatch(string value, string input)
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher(value);

            // Act
            double match = matcher.IsMatch(input);

            // Assert
            Assert.Equal(0.0, match);
        }

        [Theory]
        [InlineData("{ \"test.nested\":123 }", "{\"test\":{\"nested\":123}}")]
        [InlineData("{ \"test.nested\":[123, 456] }", "{\"test\":{\"nested\":[123, 456]}}")]
        [InlineData("{ \"test.nested\":\"value\" }", "{\"test\":{\"nested\":\"value\"}}")]
        [InlineData("{ \"['name.with.dot']\":\"value\" }", "{\"name.with.dot\":\"value\"}")]
        [InlineData("[{ \"test.nested\":\"value\" }]", "[{\"test\":{\"nested\":\"value\"}}]")]
        [InlineData("[{ \"['name.with.dot']\":\"value\" }]", "[{\"name.with.dot\":\"value\"}]")]
        public void JsonPartialWildcardMatcher_IsMatch_ValueAsJPathValidMatch(string value, string input)
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher(value);

            // Act
            double match = matcher.IsMatch(input);

            // Assert
            Assert.Equal(1.0, match);
        }

        [Theory]
        [InlineData("{ \"test.nested\":123 }", "{\"test\":{\"nested\":456}}")]
        [InlineData("{ \"test.nested\":[123, 456] }", "{\"test\":{\"nested\":[1, 2]}}")]
        [InlineData("{ \"test.nested\":\"value\" }", "{\"test\":{\"nested\":\"value1\"}}")]
        [InlineData("{ \"['name.with.dot']\":\"value\" }", "{\"name.with.dot\":\"value1\"}")]
        [InlineData("[{ \"test.nested\":\"value\" }]", "[{\"test\":{\"nested\":\"value1\"}}]")]
        [InlineData("[{ \"['name.with.dot']\":\"value\" }]", "[{\"name.with.dot\":\"value1\"}]")]
        public void JsonPartialWildcardMatcher_IsMatch_ValueAsJPathInvalidMatch(string value, string input)
        {
            // Assign
            var matcher = new JsonPartialWildcardMatcher(value);

            // Act
            double match = matcher.IsMatch(input);

            // Assert
            Assert.Equal(0.0, match);
        }
    }
}

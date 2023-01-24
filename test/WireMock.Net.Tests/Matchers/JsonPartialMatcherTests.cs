using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class JsonPartialMatcherTests
{
    [Fact]
    public void JsonPartialMatcher_GetName()
    {
        // Assign
        var matcher = new JsonPartialMatcher("{}");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("JsonPartialMatcher");
    }

    [Fact]
    public void JsonPartialMatcher_GetValue()
    {
        // Assign
        var matcher = new JsonPartialMatcher("{}");

        // Act
        object value = matcher.Value;

        // Assert
        Check.That(value).Equals("{}");
    }

    [Fact]
    public void JsonPartialMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonPartialMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonPartialMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonPartialMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithInvalidValue_And_ThrowExceptionIsFalse_Should_ReturnMismatch()
    {
        // Assign
        var matcher = new JsonPartialMatcher("");

        // Act
        double match = matcher.IsMatch(new MemoryStream());

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithInvalidValue_And_ThrowExceptionIsTrue_Should_ReturnMismatch()
    {
        // Assign
        var matcher = new JsonPartialMatcher("", false, true);

        // Act
        Action action = () => matcher.IsMatch(new MemoryStream());

        // Assert 
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new JsonPartialMatcher("");

        // Act 
        double match = matcher.IsMatch(bytes);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new JsonPartialMatcher("");

        // Act 
        double match = matcher.IsMatch(s);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new JsonPartialMatcher("");

        // Act 
        double match = matcher.IsMatch(o);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JArray()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new[] { "x", "y" });

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
    public void JsonPartialMatcher_IsMatch_JObject()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithRegexTrue()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = "^\\d+$", Name = "Test" }, false, false, true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithRegexFalse()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = "^\\d+$", Name = "Test" });

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JObject()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { id = 1, Name = "test" }, true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "NaMe", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JArrayAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("[ \"x\", \"y\" ]");

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
    public void JsonPartialMatcher_IsMatch_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_WithIgnoreCaseTrue_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectAsString_RejectOnMatch()
    {
        // Assign 
        var matcher = new JsonPartialMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject);

        // Assert 
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void JsonPartialMatcher_IsMatch_JObjectWithDateTimeOffsetAsString()
    {
        // Assign 
        var matcher = new JsonPartialMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act 
        var jObject = new JObject
        {
            { "preferredAt", new JValue("2019-11-21T10:32:53.2210009+00:00") }
        };
        double match = matcher.IsMatch(jObject);

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
    public void JsonPartialMatcher_IsMatch_StringInputValidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

        // Act
        double match = matcher.IsMatch(input);

        // Assert
        Assert.Equal(1.0, match);
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
    public void JsonPartialMatcher_IsMatch_StringInputWithInvalidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

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
    public void JsonPartialMatcher_IsMatch_ValueAsJPathValidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

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
    public void JsonPartialMatcher_IsMatch_ValueAsJPathInvalidMatch(string value, string input)
    {
        // Assign
        var matcher = new JsonPartialMatcher(value);

        // Act
        double match = matcher.IsMatch(input);

        // Assert
        Assert.Equal(0.0, match);
    }
}
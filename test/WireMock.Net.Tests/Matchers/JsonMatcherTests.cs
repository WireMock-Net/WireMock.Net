using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class JsonMatcherTests
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnumWithJsonConverter
    {
        Type1
    }

    public enum NormalEnum
    {
        Abc
    }

    public class Test1
    {
        public NormalEnum NormalEnum { get; set; }
    }

    public class Test2
    {
        public EnumWithJsonConverter EnumWithJsonConverter { get; set; }
    }

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
    public void JsonMatcher_WithInvalidStringValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonMatcher(MatchBehaviour.AcceptOnMatch, "{ \"Id\"");

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonMatcher_WithInvalidObjectValue_Should_ThrowException()
    {
        // Act
        // ReSharper disable once ObjectCreationAsStatement
        Action action = () => new JsonMatcher(MatchBehaviour.AcceptOnMatch, new MemoryStream());

        // Assert
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithInvalidValue_Should_ReturnMismatch_And_Exception_ShouldBeSet()
    {
        // Assign
        var matcher = new JsonMatcher("");

        // Act
        var result = matcher.IsMatch(new MemoryStream());

        // Assert 
        result.Score.Should().Be(MatchScores.Mismatch);
        result.Exception.Should().BeAssignableTo<JsonException>();
    }

    [Fact]
    public void JsonMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = EmptyArray<byte>.Value;
        var matcher = new JsonMatcher("");

        // Act 
        double match = matcher.IsMatch(bytes).Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new JsonMatcher("");

        // Act 
        double match = matcher.IsMatch(s).Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new JsonMatcher("");

        // Act 
        double match = matcher.IsMatch(o).Score;

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JArray()
    {
        // Assign 
        var matcher = new JsonMatcher(new[] { "x", "y" });

        // Act 
        var jArray = new JArray
        {
            "x",
            "y"
        };
        double match = matcher.IsMatch(jArray).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JObject()
    {
        // Assign 
        var matcher = new JsonMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithIgnoreCaseTrue_JObject()
    {
        // Assign 
        var matcher = new JsonMatcher(new { id = 1, Name = "test" }, true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "NaMe", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonMatcher(new { Id = 1, Name = "Test" });

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithIgnoreCaseTrue_JObjectParsed()
    {
        // Assign 
        var matcher = new JsonMatcher(new { Id = 1, Name = "TESt" }, true);

        // Act 
        var jObject = JObject.Parse("{ \"Id\" : 1, \"Name\" : \"Test\" }");
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JArrayAsString()
    {
        // Assign 
        var matcher = new JsonMatcher("[ \"x\", \"y\" ]");

        // Act 
        var jArray = new JArray
        {
            "x",
            "y"
        };
        double match = matcher.IsMatch(jArray).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonMatcher("{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_WithIgnoreCaseTrue_JObjectAsString()
    {
        // Assign 
        var matcher = new JsonMatcher("{ \"Id\" : 1, \"Name\" : \"test\" }", true);

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JObjectAsString_RejectOnMatch()
    {
        // Assign 
        var matcher = new JsonMatcher(MatchBehaviour.RejectOnMatch, "{ \"Id\" : 1, \"Name\" : \"Test\" }");

        // Act 
        var jObject = new JObject
        {
            { "Id", new JValue(1) },
            { "Name", new JValue("Test") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(0.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_JObjectWithDateTimeOffsetAsString()
    {
        // Assign 
        var matcher = new JsonMatcher("{ \"preferredAt\" : \"2019-11-21T10:32:53.2210009+00:00\" }");

        // Act 
        var jObject = new JObject
        {
            { "preferredAt", new JValue("2019-11-21T10:32:53.2210009+00:00") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void JsonMatcher_IsMatch_NormalEnum()
    {
        // Assign 
        var matcher = new JsonMatcher(new Test1 { NormalEnum = NormalEnum.Abc});

        // Act 
        var jObject = new JObject
        {
            { "NormalEnum", new JValue(0) }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        match.Should().Be(1.0);
    }

    [Fact]
    public void JsonMatcher_IsMatch_EnumWithJsonConverter()
    {
        // Assign 
        var matcher = new JsonMatcher(new Test2 { EnumWithJsonConverter = EnumWithJsonConverter.Type1 });

        // Act 
        var jObject = new JObject
        {
            { "EnumWithJsonConverter", new JValue("Type1") }
        };
        double match = matcher.IsMatch(jObject).Score;

        // Assert 
        match.Should().Be(1.0);
    }
}
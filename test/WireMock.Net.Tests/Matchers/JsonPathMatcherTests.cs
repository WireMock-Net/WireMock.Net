using System;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class JsonPathMatcherTests
{
    [Fact]
    public void JsonPathMatcher_GetName()
    {
        // Arrange
        var matcher = new JsonPathMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("JsonPathMatcher");
    }

    [Fact]
    public void JsonPathMatcher_GetPatterns()
    {
        // Arrange
        var matcher = new JsonPathMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("X");
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_ByteArray()
    {
        // Arrange
        var bytes = EmptyArray<byte>.Value;
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch(bytes);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_NullString()
    {
        // Arrange
        string? s = null;
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch(s);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_NullObject()
    {
        // Arrange
        object? o = null;
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch(o);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_String_Exception_Mismatch()
    {
        // Arrange
        var matcher = new JsonPathMatcher("xxx");

        // Act 
        double match = matcher.IsMatch("");

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_Object_Exception_Mismatch()
    {
        // Arrange
        var matcher = new JsonPathMatcher("");

        // Act 
        double match = matcher.IsMatch("x");

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_AnonymousObject()
    {
        // Arrange 
        var matcher = new JsonPathMatcher("$..[?(@.Id == 1)]");

        // Act 
        double match = matcher.IsMatch(new { Id = 1, Name = "Test" });

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    // - https://github.com/WireMock-Net/WireMock.Net/issues/965
    // - https://stackoverflow.com/questions/66922188/newtonsoft-jsonpath-with-c-sharp-syntax
    [Fact]
    public void JsonPathMatcher_IsMatch_AnonymousObject_WithNestedObject()
    {
        // Arrange
        var matcher = new JsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act 
        double match = matcher.IsMatch(new { things = new { name = "x" } });

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_String_WithNestedObject()
    {
        // Arrange
        var json = "{ \"things\": { \"name\": \"x\" } }";
        var matcher = new JsonPathMatcher("$.things[?(@.name == 'x')]");

        // Act 
        double match = matcher.IsMatch(json);

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_JObject()
    {
        // Arrange 
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
        // Arrange 
        var matcher = new JsonPathMatcher("$..[?(@.Id == 1)]");

        // Act 
        double match = matcher.IsMatch(JObject.Parse("{\"Id\":1,\"Name\":\"Test\"}"));

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JsonPathMatcher_IsMatch_RejectOnMatch()
    {
        // Arrange
        var matcher = new JsonPathMatcher(MatchBehaviour.RejectOnMatch, false, MatchOperator.Or, "$..[?(@.Id == 1)]");

        // Act
        double match = matcher.IsMatch(JObject.Parse("{\"Id\":1,\"Name\":\"Test\"}"));

        // Assert
        Check.That(match).IsEqualTo(0.0);
    }
}
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class JmesPathMatcherTests
{
    [Fact]
    public void JmesPathMatcher_GetName()
    {
        // Assign
        var matcher = new JmesPathMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("JmesPathMatcher");
    }

    [Fact]
    public void JmesPathMatcher_GetPatterns()
    {
        // Assign
        var matcher = new JmesPathMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("X");
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_ByteArray()
    {
        // Assign
        var bytes = new byte[0];
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch(bytes);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_NullString()
    {
        // Assign
        string? s = null;
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch(s);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_NullObject()
    {
        // Assign
        object? o = null;
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch(o);

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_String_Exception_Mismatch()
    {
        // Assign
        var matcher = new JmesPathMatcher("xxx");

        // Act 
        double match = matcher.IsMatch("");

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_Object_Exception_Mismatch()
    {
        // Assign
        var matcher = new JmesPathMatcher("");

        // Act 
        double match = matcher.IsMatch("x");

        // Assert 
        Check.That(match).IsEqualTo(0);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_AnonymousObject()
    {
        // Assign 
        var matcher = new JmesPathMatcher("things.name == 'RequiredThing'");

        // Act
        double match = matcher.IsMatch(new { things = new { name = "RequiredThing" } });

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_JObject()
    {
        // Assign 
        string[] patterns = { "things.x == 'RequiredThing'" };
        var matcher = new JmesPathMatcher(patterns);

        // Act
        var sub = new JObject
        {
            { "x", new JValue("RequiredThing") }
        };
        var jobject = new JObject
        {
            { "Id", new JValue(1) },
            { "things", sub }
        };
        double match = matcher.IsMatch(jobject);

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_JObject_Parsed()
    {
        // Assign 
        var matcher = new JmesPathMatcher("things.x == 'RequiredThing'");

        // Act 
        double match = matcher.IsMatch(JObject.Parse("{ \"things\": { \"x\": \"RequiredThing\" } }"));

        // Assert 
        Check.That(match).IsEqualTo(1);
    }

    [Fact]
    public void JmesPathMatcher_IsMatch_RejectOnMatch()
    {
        // Assign
        var matcher = new JmesPathMatcher(MatchBehaviour.RejectOnMatch, false, MatchOperator.Or, "things.x == 'RequiredThing'");

        // Act
        double match = matcher.IsMatch(JObject.Parse("{ \"things\": { \"x\": \"RequiredThing\" } }"));

        // Assert
        Check.That(match).IsEqualTo(0.0);
    }
}
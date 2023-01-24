using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class CSharpCodeMatcherTests
{
    [Fact]
    public void CSharpCodeMatcher_For_String_SinglePattern_IsMatch_Positive()
    {
        // Assign
        string input = "x";

        // Act
        var matcher = new CSharpCodeMatcher("return it == \"x\";");

        // Assert
        Check.That(matcher.IsMatch(input)).IsEqualTo(1.0d);
    }

    [Fact]
    public void CSharpCodeMatcher_For_String_IsMatch_Negative()
    {
        // Assign
        string input = "y";

        // Act
        var matcher = new CSharpCodeMatcher("return it == \"x\";");

        // Assert
        Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
    }

    [Fact]
    public void CSharpCodeMatcher_For_String_IsMatch_RejectOnMatch()
    {
        // Assign
        string input = "x";

        // Act
        var matcher = new CSharpCodeMatcher(MatchBehaviour.RejectOnMatch, MatchOperator.Or, "return it == \"x\";");

        // Assert
        Check.That(matcher.IsMatch(input)).IsEqualTo(0.0d);
    }

    [Fact]
    public void CSharpCodeMatcher_For_Object_IsMatch()
    {
        // Assign
        var input = new
        {
            Id = 9,
            Name = "Test"
        };

        // Act
        var matcher = new CSharpCodeMatcher("return it.Id > 1 && it.Name == \"Test\";");
        double match = matcher.IsMatch(input);

        // Assert
        Assert.Equal(1.0, match);
    }

    [Fact]
    public void CSharpCodeMatcher_GetName()
    {
        // Assign
        var matcher = new CSharpCodeMatcher("x");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("CSharpCodeMatcher");
    }

    [Fact]
    public void CSharpCodeMatcher_GetPatterns()
    {
        // Assign
        var matcher = new CSharpCodeMatcher("x");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("x");
    }
}
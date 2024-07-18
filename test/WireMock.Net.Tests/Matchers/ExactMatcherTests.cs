// Copyright © WireMock.Net

using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers;

public class ExactMatcherTests
{
    [Fact]
    public void ExactMatcher_GetName()
    {
        // Assign
        var matcher = new ExactMatcher("X");

        // Act
        string name = matcher.Name;

        // Assert
        Check.That(name).Equals("ExactMatcher");
    }

    [Fact]
    public void ExactMatcher_GetPatterns()
    {
        // Assign
        var matcher = new ExactMatcher("X");

        // Act
        var patterns = matcher.GetPatterns();

        // Assert
        Check.That(patterns).ContainsExactly("X");
    }

    [Fact]
    public void ExactMatcher_IsMatch_IgnoreCase()
    {
        // Assign
        var matcher = new ExactMatcher(true, "x");

        // Act
        double result = matcher.IsMatch("X").Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void ExactMatcher_IsMatch_WithSinglePattern_ReturnsMatch1_0()
    {
        // Assign
        var matcher = new ExactMatcher("x");

        // Act
        double result = matcher.IsMatch("x").Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void ExactMatcher_IsMatch_WithSinglePattern_ReturnsMatch0_0()
    {
        // Assign
        var matcher = new ExactMatcher("x");

        // Act
        double result = matcher.IsMatch("y").Score;

        // Assert
        Check.That(result).IsEqualTo(0.0);
    }

    [Fact]
    public void ExactMatcher_IsMatch_WithMultiplePatterns_Or_ReturnsMatch_1_0()
    {
        // Assign
        var matcher = new ExactMatcher("x", "y");

        // Act
        double result = matcher.IsMatch("x").Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void ExactMatcher_IsMatch_WithMultiplePatterns_And_ReturnsMatch_0_0()
    {
        // Assign
        var matcher = new ExactMatcher("x", "y");

        // Act
        double result = matcher.IsMatch("x").Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Theory]
    [InlineData(MatchOperator.Or, 1.0d)]
    [InlineData(MatchOperator.And, 0.0d)]
    [InlineData(MatchOperator.Average, 0.5d)]
    public void ExactMatcher_IsMatch_WithMultiplePatterns_Average_ReturnsMatch(MatchOperator matchOperator, double score)
    {
        // Assign
        var matcher = new ExactMatcher(MatchBehaviour.AcceptOnMatch, false, matchOperator, "x", "y");

        // Act
        double result = matcher.IsMatch("x").Score;

        // Assert
        Check.That(result).IsEqualTo(score);
    }

    [Fact]
    public void ExactMatcher_IsMatch_SinglePattern()
    {
        // Assign
        var matcher = new ExactMatcher("cat");

        // Act
        double result = matcher.IsMatch("caR").Score;

        // Assert
        Check.That(result).IsEqualTo(0.0);
    }

    [Fact]
    public void ExactMatcher_IsMatch_SinglePattern_AcceptOnMatch()
    {
        // Assign
        var matcher = new ExactMatcher(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, "cat");

        // Act
        double result = matcher.IsMatch("cat").Score;

        // Assert
        Check.That(result).IsEqualTo(1.0);
    }

    [Fact]
    public void ExactMatcher_IsMatch_SinglePattern_RejectOnMatch()
    {
        // Assign
        var matcher = new ExactMatcher(MatchBehaviour.RejectOnMatch, false, MatchOperator.Or, "cat");

        // Act
        double result = matcher.IsMatch("cat").Score;

        // Assert
        Check.That(result).IsEqualTo(0.0);
    }
}
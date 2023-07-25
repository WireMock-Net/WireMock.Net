using System;
using AnyOfTypes;
using FluentAssertions;
using Moq;
using NFluent;
using WireMock.Admin.Mappings;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public class MatcherMapperTests
{
    private readonly WireMockServerSettings _settings = new();
    private readonly MatcherMapper _sut;

    public MatcherMapperTests()
    {
        _sut = new MatcherMapper(_settings);
    }

    [Fact]
    public void MatcherMapper_Map_IMatcher_Null()
    {
        // Act
        var model = _sut.Map((IMatcher?)null);

        // Assert
        model.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_IMatchers_Null()
    {
        // Act
        var model = _sut.Map((IMatcher[]?)null);

        // Assert
        model.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_IMatchers()
    {
        // Assign
        var matcherMock1 = new Mock<IStringMatcher>();
        var matcherMock2 = new Mock<IStringMatcher>();

        // Act
        var models = _sut.Map(new[] { matcherMock1.Object, matcherMock2.Object });

        // Assert
        models.Should().HaveCount(2);
    }

    [Fact]
    public void MatcherMapper_Map_IStringMatcher()
    {
        // Assign
        var matcherMock = new Mock<IStringMatcher>();
        matcherMock.Setup(m => m.Name).Returns("test");
        matcherMock.Setup(m => m.GetPatterns()).Returns(new AnyOf<string, StringPattern>[] { "p1", "p2" });

        // Act
        var model = _sut.Map(matcherMock.Object)!;

        // Assert
        model.IgnoreCase.Should().BeNull();
        model.Name.Should().Be("test");
        model.Pattern.Should().BeNull();
        model.Patterns.Should().HaveCount(2)
            .And.Contain("p1")
            .And.Contain("p2");
    }

    [Fact]
    public void MatcherMapper_Map_IStringMatcher_With_PatternAsFile()
    {
        // Arrange
        var pattern = new StringPattern { Pattern = "p", PatternAsFile = "pf" };

        var matcherMock = new Mock<IStringMatcher>();
        matcherMock.Setup(m => m.Name).Returns("test");
        matcherMock.Setup(m => m.GetPatterns()).Returns(new AnyOf<string, StringPattern>[] { pattern });

        // Act
        var model = _sut.Map(matcherMock.Object)!;

        // Assert
        model.IgnoreCase.Should().BeNull();
        model.Name.Should().Be("test");
        model.Pattern.Should().Be("p");
        model.Patterns.Should().BeNull();
        model.PatternAsFile.Should().Be("pf");
    }

    [Fact]
    public void MatcherMapper_Map_IIgnoreCaseMatcher()
    {
        // Assign
        var matcherMock = new Mock<IIgnoreCaseMatcher>();
        matcherMock.Setup(m => m.IgnoreCase).Returns(true);

        // Act
        var model = _sut.Map(matcherMock.Object)!;

        // Assert
        model.IgnoreCase.Should().BeTrue();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_Null()
    {
        // Act
        var result = _sut.Map((MatcherModel?)null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_Exception()
    {
        // Assign
        var model = new MatcherModel { Name = "test" };

        // Act and Assert
        Check.ThatCode(() => _sut.Map(model)).Throws<NotSupportedException>();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_LinqMatcher_Pattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "LinqMatcher",
            Pattern = "p"
        };

        // Act
        var matcher = (LinqMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.GetPatterns().Should().Contain("p");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_LinqMatcher_Patterns()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "LinqMatcher",
            Patterns = new[] { "p1", "p2" }
        };

        // Act
        var matcher = (LinqMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.GetPatterns().Should().Contain("p1", "p2");
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_1_Value_As_String()
    {
        // Assign
        var pattern = "{ \"post1\": \"value1\", \"post2\": \"value2\" }";
        var patterns = new[] { pattern };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_2_Values_As_String()
    {
        // Assign
        var pattern1 = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var pattern2 = "{ \"post1\": \"value1\", \"post2\": \"value2\" }";
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Pattern_As_Object()
    {
        // Assign
        var pattern = new { AccountIds = new[] { 1, 2, 3 } };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_1_Value_As_Object()
    {
        // Assign
        object pattern = new { post1 = "value1", post2 = "value2" };
        var patterns = new[] { pattern };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonMatcher_Patterns_2_Values_As_Object()
    {
        // Assign
        object pattern1 = new { AccountIds = new[] { 1, 2, 3 } };
        object pattern2 = new { post1 = "value1", post2 = "value2" };
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Pattern_As_String()
    {
        // Assign
        var pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Patterns_As_String()
    {
        // Assign
        var pattern1 = "{ \"AccountIds\": [ 1, 2, 3 ] }";
        var pattern2 = "{ \"X\": \"x\" }";
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = patterns
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Pattern_As_Object()
    {
        // Assign
        var pattern = new { AccountIds = new[] { 1, 2, 3 } };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_Patterns_As_Object()
    {
        // Assign
        object pattern1 = new { AccountIds = new[] { 1, 2, 3 } };
        object pattern2 = new { X = "x" };
        var patterns = new[] { pattern1, pattern2 };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Patterns = patterns
        };

        // Act
        var matcher = (JsonMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(patterns);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialMatcher_StringPattern_With_PatternAsFile()
    {
        // Assign
        var pattern = new StringPattern { Pattern = "{ \"AccountIds\": [ 1, 2, 3 ] }", PatternAsFile = "pf" };
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Pattern = pattern,
            Regex = true
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
        matcher.Regex.Should().BeTrue();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_JsonPartialWildcardMatcher_Patterns_As_Object()
    {
        // Assign
        object pattern = new { X = "*" };
        var model = new MatcherModel
        {
            Name = "JsonPartialWildcardMatcher",
            Pattern = pattern,
            Regex = false
        };

        // Act
        var matcher = (JsonPartialWildcardMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.Value.Should().BeEquivalentTo(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_ExactMatcher_Pattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactMatcher",
            Pattern = "p",
            IgnoreCase = true
        };

        // Act
        var matcher = (ExactMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.GetPatterns().Should().Contain("p");
        matcher.IgnoreCase.Should().BeTrue();
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_NotNullOrEmptyMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "NotNullOrEmptyMatcher",
            RejectOnMatch = true
        };

        // Act
        var matcher = _sut.Map(model)!;

        // Assert
        matcher.Should().BeAssignableTo<NotNullOrEmptyMatcher>();
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.RejectOnMatch);
    }
}
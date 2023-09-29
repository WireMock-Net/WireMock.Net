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

#if MIMEKIT
    [Fact]
    public void MatcherMapper_Map_MimePartMatcher()
    {
        // Arrange
        var bytes = Convert.FromBase64String("c3RlZg==");
        var imagePngContentTypeMatcher = new ContentTypeMatcher("image/png");
        var imagePngContentDispositionMatcher = new ExactMatcher("attachment; filename=\"image.png\"");
        var imagePngContentTransferEncodingMatcher = new ExactMatcher("base64");
        var imagePngContentMatcher = new ExactObjectMatcher(bytes);
        var imagePngMatcher = new MimePartMatcher(MatchBehaviour.AcceptOnMatch, imagePngContentTypeMatcher, imagePngContentDispositionMatcher, imagePngContentTransferEncodingMatcher, imagePngContentMatcher);

        // Act
        var model = _sut.Map(imagePngMatcher)!;

        // Assert
        model.Name.Should().Be(nameof(MimePartMatcher));
        model.MatchOperator.Should().BeNull();
        model.RejectOnMatch.Should().BeNull();

        model.ContentTypeMatcher!.Name.Should().Be(nameof(ContentTypeMatcher));
        model.ContentTypeMatcher.Pattern.Should().Be("image/png");

        model.ContentDispositionMatcher!.Name.Should().Be(nameof(ExactMatcher));
        model.ContentDispositionMatcher.Pattern.Should().Be("attachment; filename=\"image.png\"");

        model.ContentTransferEncodingMatcher!.Name.Should().Be(nameof(ExactMatcher));
        model.ContentTransferEncodingMatcher.Pattern.Should().Be("base64");

        model.ContentMatcher!.Name.Should().Be(nameof(ExactObjectMatcher));
        model.ContentMatcher.Pattern.Should().Be(bytes);
    }
#endif

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

#if MIMEKIT
    [Fact]
    public void MatcherMapper_Map_MatcherModel_MimePartMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "MimePartMatcher",
            ContentMatcher = new MatcherModel
            {
                Name = "ExactMatcher",
                Pattern = "x"
            },
            ContentDispositionMatcher = new MatcherModel
            {
                Name = "WildcardMatcher",
                Pattern = "y"
            },
            ContentTransferEncodingMatcher = new MatcherModel
            {
                Name = "RegexMatcher",
                Pattern = "z"
            },
            ContentTypeMatcher = new MatcherModel
            {
                Name = "ContentTypeMatcher",
                Pattern = "text/json"
            }
        };

        // Act
        var matcher = (MimePartMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.ContentMatcher.Should().BeAssignableTo<ExactMatcher>().Which.GetPatterns().Should().ContainSingle("x");
        matcher.ContentDispositionMatcher.Should().BeAssignableTo<WildcardMatcher>().Which.GetPatterns().Should().ContainSingle("y");
        matcher.ContentTransferEncodingMatcher.Should().BeAssignableTo<RegexMatcher>().Which.GetPatterns().Should().ContainSingle("z");
        matcher.ContentTypeMatcher.Should().BeAssignableTo<ContentTypeMatcher>().Which.GetPatterns().Should().ContainSingle("text/json");
    }
#endif

    [Fact]
    public void MatcherMapper_Map_MatcherModel_XPathMatcher_WithXmlNamespaces_As_String()
    {
        // Assign
        var pattern = "/s:Envelope/s:Body/*[local-name()='QueryRequest']";
        var model = new MatcherModel
        {
            Name = "XPathMatcher",
            Pattern = pattern,
            XmlNamespaceMap = new[]
            {
                new XmlNamespace { Prefix = "s", Uri = "http://schemas.xmlsoap.org/soap/envelope/" }
            }
        };

        // Act
        var matcher = (XPathMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.XmlNamespaceMap.Should().NotBeNull();
        matcher.XmlNamespaceMap.Should().HaveCount(1);
    }

    [Fact]
    public void MatcherMapper_Map_MatcherModel_XPathMatcher_WithoutXmlNamespaces_As_String()
    {
        // Assign
        var pattern = "/s:Envelope/s:Body/*[local-name()='QueryRequest']";
        var model = new MatcherModel
        {
            Name = "XPathMatcher",
            Pattern = pattern
        };

        // Act
        var matcher = (XPathMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.XmlNamespaceMap.Should().NotBeNull();
        matcher.XmlNamespaceMap.Should().HaveCount(0);
    }
}
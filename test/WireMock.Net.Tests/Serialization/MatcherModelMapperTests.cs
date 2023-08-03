using System;
using System.Collections.Generic;
using AnyOfTypes;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Newtonsoft.Json;
using NFluent;
using WireMock.Admin.Mappings;
using WireMock.Handlers;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public class MatcherModelMapperTests
{
    private readonly WireMockServerSettings _settings = new();

    private readonly MatcherMapper _sut;

    public MatcherModelMapperTests()
    {
        _sut = new MatcherMapper(_settings);
    }

    [Fact]
    public void MatcherModelMapper_Map_CSharpCodeMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "CSharpCodeMatcher",
            Patterns = new[] { "return it == \"x\";" }
        };
        var sut = new MatcherMapper(new WireMockServerSettings { AllowCSharpCodeMatcher = true });

        // Act 1
        var matcher1 = (ICSharpCodeMatcher)sut.Map(model)!;

        // Assert 1
        matcher1.Should().NotBeNull();
        matcher1.IsMatch("x").Should().Be(1.0d);

        // Act 2
        var matcher2 = (ICSharpCodeMatcher)sut.Map(model)!;

        // Assert 2
        matcher2.Should().NotBeNull();
        matcher2.IsMatch("x").Should().Be(1.0d);
    }

    [Fact]
    public void MatcherModelMapper_Map_CSharpCodeMatcher_NotAllowed_ThrowsException()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "CSharpCodeMatcher",
            Patterns = new[] { "x" }
        };
        var sut = new MatcherMapper(new WireMockServerSettings { AllowCSharpCodeMatcher = false });

        // Act
        Action action = () => sut.Map(model);

        // Assert
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void MatcherModelMapper_Map_Null()
    {
        // Act
        IMatcher matcher = _sut.Map((MatcherModel?)null)!;

        // Assert
        Check.That(matcher).IsNull();
    }

    [Fact]
    public void MatcherModelMapper_Map_ExactMatcher_Pattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactMatcher",
            Patterns = new[] { "x" }
        };

        // Act
        var matcher = (ExactMatcher)_sut.Map(model)!;

        // Assert
        matcher.GetPatterns().Should().ContainSingle("x");
    }

    [Fact]
    public void MatcherModelMapper_Map_ExactMatcher_Patterns()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactMatcher",
            Patterns = new[] { "x", "y" }
        };

        // Act
        var matcher = (ExactMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
    }

    [Fact]
    public void MatcherModelMapper_Map_JsonPartialMatcher_RegexFalse()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Regex = false,
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.IgnoreCase.Should().BeFalse();
        matcher.Value.Should().Be(pattern);
        matcher.Regex.Should().BeFalse();
    }

    [Fact]
    public void MatcherModelMapper_Map_JsonPartialMatcher_RegexTrue()
    {
        // Assign
        var pattern = "{ \"x\": 1 }";
        var model = new MatcherModel
        {
            Name = "JsonPartialMatcher",
            Regex = true,
            Pattern = pattern
        };

        // Act
        var matcher = (JsonPartialMatcher)_sut.Map(model)!;

        // Assert
        matcher.MatchBehaviour.Should().Be(MatchBehaviour.AcceptOnMatch);
        matcher.IgnoreCase.Should().BeFalse();
        matcher.Value.Should().Be(pattern);
        matcher.Regex.Should().BeTrue();
    }

    [Fact]
    public void MatcherModelMapper_Map_ExactObjectMatcher_ValidBase64StringPattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactObjectMatcher",
            Patterns = new object[] { "c3RlZg==" }
        };

        // Act
        var matcher = (ExactObjectMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.ValueAsBytes).ContainsExactly(new byte[] { 115, 116, 101, 102 });
    }

    [Fact]
    public void MatcherModelMapper_Map_ExactObjectMatcher_InvalidBase64StringPattern()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "ExactObjectMatcher",
            Patterns = new object[] { "_" }
        };

        // Act & Assert
        Check.ThatCode(() => _sut.Map(model)).Throws<ArgumentException>();
    }

    [Theory]
    [InlineData(MatchOperator.Or, 1.0d)]
    [InlineData(MatchOperator.And, 0.0d)]
    [InlineData(MatchOperator.Average, 0.5d)]
    public void MatcherModelMapper_Map_RegexMatcher(MatchOperator matchOperator, double expected)
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "RegexMatcher",
            Patterns = new[] { "x", "y" },
            IgnoreCase = true,
            MatchOperator = matchOperator.ToString()
        };

        // Act
        var matcher = (RegexMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
        Check.That(matcher.IsMatch("X")).IsEqualTo(expected);
    }

    [Theory]
    [InlineData(MatchOperator.Or, 1.0d)]
    [InlineData(MatchOperator.And, 0.0d)]
    [InlineData(MatchOperator.Average, 0.5d)]
    public void MatcherModelMapper_Map_WildcardMatcher_IgnoreCase(MatchOperator matchOperator, double expected)
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "WildcardMatcher",
            Patterns = new[] { "x", "y" },
            IgnoreCase = true,
            MatchOperator = matchOperator.ToString()
        };

        // Act
        var matcher = (WildcardMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
        Check.That(matcher.IsMatch("X")).IsEqualTo(expected);
    }

    [Fact]
    public void MatcherModelMapper_Map_WildcardMatcher_With_PatternAsFile()
    {
        // Arrange
        var file = "c:\\test.txt";
        var fileContent = "c";
        var stringPattern = new StringPattern
        {
            Pattern = fileContent,
            PatternAsFile = file
        };
        var fileSystemHandleMock = new Mock<IFileSystemHandler>();
        fileSystemHandleMock.Setup(f => f.ReadFileAsString(file)).Returns(fileContent);

        var model = new MatcherModel
        {
            Name = "WildcardMatcher",
            PatternAsFile = file
        };

        var settings = new WireMockServerSettings
        {
            FileSystemHandler = fileSystemHandleMock.Object
        };
        var sut = new MatcherMapper(settings);

        // Act
        var matcher = (WildcardMatcher)sut.Map(model)!;

        // Assert
        matcher.GetPatterns().Should().HaveCount(1).And.Contain(new AnyOf<string, StringPattern>(stringPattern));
        matcher.IsMatch("c").Should().Be(1.0d);
    }

    [Fact]
    public void MatcherModelMapper_Map_SimMetricsMatcher()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "SimMetricsMatcher",
            Pattern = "x"
        };

        // Act
        var matcher = (SimMetricsMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x");
    }

    [Fact]
    public void MatcherModelMapper_Map_SimMetricsMatcher_BlockDistance()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "SimMetricsMatcher.BlockDistance",
            Pattern = "x"
        };

        // Act
        var matcher = (SimMetricsMatcher)_sut.Map(model)!;

        // Assert
        Check.That(matcher.GetPatterns()).ContainsExactly("x");
    }

    [Fact]
    public void MatcherModelMapper_Map_SimMetricsMatcher_Throws1()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "error",
            Pattern = "x"
        };

        // Act
        Check.ThatCode(() => _sut.Map(model)).Throws<NotSupportedException>();
    }

    [Fact]
    public void MatcherModelMapper_Map_SimMetricsMatcher_Throws2()
    {
        // Assign
        var model = new MatcherModel
        {
            Name = "SimMetricsMatcher.error",
            Pattern = "x"
        };

        // Act
        Check.ThatCode(() => _sut.Map(model)).Throws<NotSupportedException>();
    }

    [Fact]
    public void MatcherModelMapper_Map_MatcherModelToCustomMatcher()
    {
        // Arrange
        var patternModel = new CustomPathParamMatcherModel("/customer/{customerId}/document/{documentId}",
            new Dictionary<string, string>(2)
            {
                { "customerId", @"^[0-9]+$" },
                { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
            });
        var model = new MatcherModel
        {
            Name = nameof(CustomPathParamMatcher),
            Pattern = JsonConvert.SerializeObject(patternModel)
        };

        var settings = new WireMockServerSettings();
        settings.CustomMatcherMappings = settings.CustomMatcherMappings ?? new Dictionary<string, Func<MatcherModel, IMatcher>>();
        settings.CustomMatcherMappings[nameof(CustomPathParamMatcher)] = matcherModel =>
        {
            var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)matcherModel.Pattern!)!;
            return new CustomPathParamMatcher(
                matcherModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                matcherParams.Path,
                matcherParams.PathParams
            );
        };
        var sut = new MatcherMapper(settings);

        // Act
        var matcher = sut.Map(model) as CustomPathParamMatcher;

        // Assert
        matcher.Should().NotBeNull();
    }

    [Fact]
    public void MatcherModelMapper_Map_CustomMatcherToMatcherModel()
    {
        // Arrange
        var matcher = new CustomPathParamMatcher("/customer/{customerId}/document/{documentId}",
            new Dictionary<string, string>(2)
            {
                { "customerId", @"^[0-9]+$" },
                { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
            });

        // Act
        var model = _sut.Map(matcher)!;

        // Assert
        using (new AssertionScope())
        {
            model.Should().NotBeNull();
            model.Name.Should().Be(nameof(CustomPathParamMatcher));

            var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)model.Pattern!)!;
            matcherParams.Path.Should().Be("/customer/{customerId}/document/{documentId}");
            matcherParams.PathParams.Should().BeEquivalentTo(new Dictionary<string, string>(2)
            {
                { "customerId", @"^[0-9]+$" },
                { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
            });
        }
    }
}
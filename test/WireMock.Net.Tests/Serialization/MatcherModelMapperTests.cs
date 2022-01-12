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

namespace WireMock.Net.Tests.Serialization
{
    public class MatcherModelMapperTests
    {
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

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
            var matcher1 = (ICSharpCodeMatcher)sut.Map(model);

            // Assert 1
            matcher1.Should().NotBeNull();
            matcher1.IsMatch("x").Should().Be(1.0d);

            // Act 2
            var matcher2 = (ICSharpCodeMatcher)sut.Map(model);

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
            IMatcher matcher = _sut.Map((MatcherModel)null);

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
            var matcher = (ExactMatcher)_sut.Map(model);

            // Assert
            matcher.GetPatterns().Should().ContainSingle("x");
            matcher.ThrowException.Should().BeFalse();
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
            var matcher = (ExactMatcher)_sut.Map(model);

            // Assert
            Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
        }

        [Theory]
        [InlineData(nameof(LinqMatcher))]
        [InlineData(nameof(ExactMatcher))]
        [InlineData(nameof(ExactObjectMatcher))]
        [InlineData(nameof(RegexMatcher))]
        [InlineData(nameof(JsonMatcher))]
        [InlineData(nameof(JsonPathMatcher))]
        [InlineData(nameof(JmesPathMatcher))]
        [InlineData(nameof(XPathMatcher))]
        [InlineData(nameof(WildcardMatcher))]
        [InlineData(nameof(ContentTypeMatcher))]
        [InlineData(nameof(SimMetricsMatcher))]
        public void MatcherModelMapper_Map_ThrowExceptionWhenMatcherFails_True(string name)
        {
            // Assign
            var settings = new WireMockServerSettings
            {
                ThrowExceptionWhenMatcherFails = true
            };
            var sut = new MatcherMapper(settings);
            var model = new MatcherModel
            {
                Name = name,
                Patterns = new[] { "" }
            };

            // Act
            var matcher = sut.Map(model);

            // Assert
            matcher.ThrowException.Should().BeTrue();
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
            var matcher = (ExactObjectMatcher)_sut.Map(model);

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

        [Fact]
        public void MatcherModelMapper_Map_RegexMatcher()
        {
            // Assign
            var model = new MatcherModel
            {
                Name = "RegexMatcher",
                Patterns = new[] { "x", "y" },
                IgnoreCase = true
            };

            // Act
            var matcher = (RegexMatcher)_sut.Map(model);

            // Assert
            Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
            Check.That(matcher.IsMatch("X")).IsEqualTo(0.5d);
        }

        [Fact]
        public void MatcherModelMapper_Map_WildcardMatcher_IgnoreCase()
        {
            // Assign
            var model = new MatcherModel
            {
                Name = "WildcardMatcher",
                Patterns = new[] { "x", "y" },
                IgnoreCase = true
            };

            // Act
            var matcher = (WildcardMatcher)_sut.Map(model);

            // Assert
            Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
            Check.That(matcher.IsMatch("X")).IsEqualTo(0.5d);
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
            var matcher = (WildcardMatcher)sut.Map(model);

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
            var matcher = (SimMetricsMatcher)_sut.Map(model);

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
            var matcher = (SimMetricsMatcher)_sut.Map(model);

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
            settings.CustomMatcherMapping = settings.CustomMatcherMapping ?? new Dictionary<string, Func<MatcherModel, IMatcher>>();
            settings.CustomMatcherMapping[nameof(CustomPathParamMatcher)] = matcherModel =>
            {
                var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)matcherModel.Pattern);
                return new CustomPathParamMatcher(
                    matcherModel.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch,
                    matcherParams.Path, matcherParams.PathParams,
                    settings.ThrowExceptionWhenMatcherFails == true
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
            var model = _sut.Map(matcher);

            // Assert
            using (new AssertionScope())
            {
                model.Should().NotBeNull();
                model.Name.Should().Be(nameof(CustomPathParamMatcher));
                var matcherParams = JsonConvert.DeserializeObject<CustomPathParamMatcherModel>((string)model.Pattern);
                matcherParams.Path.Should().Be("/customer/{customerId}/document/{documentId}");
                matcherParams.PathParams.Should().BeEquivalentTo(new Dictionary<string, string>(2)
                {
                    { "customerId", @"^[0-9]+$" },
                    { "documentId", @"^[0-9a-zA-Z\-\_]+\.[a-zA-Z]+$" }
                });
            }
        }
    }
}
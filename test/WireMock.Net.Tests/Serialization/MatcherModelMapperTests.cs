using System;
using NFluent;
using WireMock.Admin.Mappings;
using WireMock.Matchers;
using WireMock.Serialization;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class MatcherModelMapperTests
    {
        [Fact]
        public void MatcherModelMapper_Map_Null()
        {
            // Act
            IMatcher matcher = MatcherMapper.Map((MatcherModel)null);

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
            var matcher = (ExactMatcher)MatcherMapper.Map(model);

            // Assert
            Check.That(matcher.GetPatterns()).ContainsExactly("x");
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
            var matcher = (ExactMatcher)MatcherMapper.Map(model);

            // Assert
            Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
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
            var matcher = (RegexMatcher)MatcherMapper.Map(model);

            // Assert
            Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
            Check.That(matcher.IsMatch("X")).IsEqualTo(0.5d);
        }

        [Fact]
        public void MatcherModelMapper_Map_WildcardMatcher()
        {
            // Assign
            var model = new MatcherModel
            {
                Name = "WildcardMatcher",
                Patterns = new[] { "x", "y" },
                IgnoreCase = true
            };

            // Act
            var matcher = (WildcardMatcher)MatcherMapper.Map(model);

            // Assert
            Check.That(matcher.GetPatterns()).ContainsExactly("x", "y");
            Check.That(matcher.IsMatch("X")).IsEqualTo(0.5d);
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
            var matcher = (SimMetricsMatcher)MatcherMapper.Map(model);

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
            var matcher = (SimMetricsMatcher)MatcherMapper.Map(model);

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
            Check.ThatCode(() => MatcherMapper.Map(model)).Throws<NotSupportedException>();
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
            Check.ThatCode(() => MatcherMapper.Map(model)).Throws<NotSupportedException>();
        }
    }
}
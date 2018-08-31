using System;
using NFluent;
using WireMock.Admin.Mappings;
using WireMock.Matchers;
using WireMock.Serialization;
using Xunit;

namespace WireMock.Net.Tests
{
    public class MatcherMapperTests
    {
        [Fact]
        public void MatcherMapper_Map_MatcherModel_Null()
        {
            // Act
            var result = MatcherMapper.Map((MatcherModel)null);

            // Assert
            Check.That(result).IsNull();
        }

        [Fact]
        public void MatcherMapper_Map_MatcherModel_Exception()
        {
            // Assign
            var model = new MatcherModel { Name = "test" };

            // Act and Assert
            Check.ThatCode(() => MatcherMapper.Map(model)).Throws<NotSupportedException>();
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
            var matcher = MatcherMapper.Map(model) as LinqMatcher;

            // Assert
            Check.That(matcher).IsNotNull();
            Check.That(matcher.MatchBehaviour).IsEqualTo(MatchBehaviour.AcceptOnMatch);
            Check.That(matcher.GetPatterns()).ContainsExactly("p");
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
            var matcher = MatcherMapper.Map(model) as LinqMatcher;

            // Assert
            Check.That(matcher).IsNotNull();
            Check.That(matcher.MatchBehaviour).IsEqualTo(MatchBehaviour.AcceptOnMatch);
            Check.That(matcher.GetPatterns()).Contains(new[] { "p1", "p2" });
        }

        [Fact]
        public void MatcherMapper_Map_IMatcher_Null()
        {
            // Act
            var result = MatcherMapper.Map((IMatcher)null);

            // Assert
            Check.That(result).IsNull();
        }

        [Fact]
        public void MatcherMapper_Map_IMatcher_LinqMatcher_Pattern()
        {
            // Assign
            var matcher = new LinqMatcher(MatchBehaviour.AcceptOnMatch, "p");

            // Act
            var result = MatcherMapper.Map(matcher);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Name).IsEqualTo("LinqMatcher");
            Check.That(result.IgnoreCase).IsNull();
            Check.That(result.Pattern).IsEqualTo("p");
            Check.That(result.Patterns).IsNull();
        }
    }
}
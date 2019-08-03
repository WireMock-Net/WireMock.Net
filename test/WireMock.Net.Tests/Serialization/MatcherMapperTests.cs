using Moq;
using NFluent;
using System;
using WireMock.Matchers;
using WireMock.Models.Mappings;
using WireMock.Serialization;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class MatcherMapperTests
    {
        [Fact]
        public void MatcherMapper_Map_IMatcher_Null()
        {
            // Act
            var model = MatcherMapper.Map((IMatcher)null);

            // Assert
            Check.That(model).IsNull();
        }

        [Fact]
        public void MatcherMapper_Map_IMatchers_Null()
        {
            // Act
            var model = MatcherMapper.Map((IMatcher[])null);

            // Assert
            Check.That(model).IsNull();
        }

        [Fact]
        public void MatcherMapper_Map_IMatchers()
        {
            // Assign
            var matcherMock1 = new Mock<IStringMatcher>();
            var matcherMock2 = new Mock<IStringMatcher>();

            // Act
            var models = MatcherMapper.Map(new[] { matcherMock1.Object, matcherMock2.Object });

            // Assert
            Check.That(models).HasSize(2);
        }

        [Fact]
        public void MatcherMapper_Map_IStringMatcher()
        {
            // Assign
            var matcherMock = new Mock<IStringMatcher>();
            matcherMock.Setup(m => m.Name).Returns("test");
            matcherMock.Setup(m => m.GetPatterns()).Returns(new[] { "p1", "p2" });

            // Act
            var model = MatcherMapper.Map(matcherMock.Object);

            // Assert
            Check.That(model.IgnoreCase).IsNull();
            Check.That(model.Name).Equals("test");
            Check.That(model.Pattern).IsNull();
            Check.That(model.Patterns).ContainsExactly("p1", "p2");
        }

        [Fact]
        public void MatcherMapper_Map_IIgnoreCaseMatcher()
        {
            // Assign
            var matcherMock = new Mock<IIgnoreCaseMatcher>();
            matcherMock.Setup(m => m.IgnoreCase).Returns(true);

            // Act
            var model = MatcherMapper.Map(matcherMock.Object);

            // Assert
            Check.That(model.IgnoreCase).Equals(true);
        }

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
            var matcher = (LinqMatcher)MatcherMapper.Map(model);

            // Assert
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
            var matcher = (LinqMatcher)MatcherMapper.Map(model);

            // Assert
            Check.That(matcher.MatchBehaviour).IsEqualTo(MatchBehaviour.AcceptOnMatch);
            Check.That(matcher.GetPatterns()).ContainsExactly("p1", "p2");
        }
    }
}
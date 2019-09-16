﻿using System.Linq;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class RequestMatchResultTests
    {
        [Fact]
        public void CompareTo_WithDifferentAverageScore_ReturnsBestMatch()
        {
            // Arrange
            var result1 = new RequestMatchResult();
            result1.AddScore(typeof(WildcardMatcher), 1);
            result1.AddScore(typeof(WildcardMatcher), 0.9);

            var result2 = new RequestMatchResult();
            result2.AddScore(typeof(LinqMatcher), 1);
            result2.AddScore(typeof(LinqMatcher), 1);

            var results = new[] { result1, result2 };

            // Act
            var best = results.OrderBy(x => x).First();

            // Assert
            best.Should().Be(result2);
        }

        [Fact]
        public void CompareTo_WithSameAverageScoreButMoreMatchers_ReturnsMatchWithMoreMatchers()
        {
            // Arrange
            var result1 = new RequestMatchResult();
            result1.AddScore(typeof(WildcardMatcher), 1);
            result1.AddScore(typeof(WildcardMatcher), 1);

            var result2 = new RequestMatchResult();
            result2.AddScore(typeof(LinqMatcher), 1);
            result2.AddScore(typeof(LinqMatcher), 1);
            result2.AddScore(typeof(LinqMatcher), 1);

            var results = new[] { result1, result2 };

            // Act
            var best = results.OrderBy(x => x).First();

            // Assert
            best.Should().Be(result2);
        }
    }
}
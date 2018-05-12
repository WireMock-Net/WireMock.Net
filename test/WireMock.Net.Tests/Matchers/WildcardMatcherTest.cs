using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class WildcardMatcherTest
    {
        [Fact]
        public void WildcardMatcher_IsMatch_Positive()
        {
            var tests = new[]
            {
                new {p = "*", i = ""},
                new {p = "?", i = " "},
                new {p = "*", i = "a"},
                new {p = "*", i = "ab"},
                new {p = "?", i = "a"},
                new {p = "*?", i = "abc"},
                new {p = "?*", i = "abc"},
                new {p = "abc", i = "abc"},
                new {p = "abc*", i = "abc"},
                new {p = "abc*", i = "abcd"},
                new {p = "*abc*", i = "abc"},
                new {p = "*a*bc*", i = "abc"},
                new {p = "*a*b?", i = "aXXXbc"}
            };

            foreach (var test in tests)
            {
                var matcher = new WildcardMatcher(test.p);
                Check.That(matcher.IsMatch(test.i)).IsEqualTo(1.0d);
            }
        }

        [Fact]
        public void WildcardMatcher_IsMatch_Negative()
        {
            var tests = new[]
            {
                new {p = "*a", i = ""},
                new {p = "a*", i = ""},
                new {p = "?", i = ""},
                new {p = "*b*", i = "a"},
                new {p = "b*a", i = "ab"},
                new {p = "??", i = "a"},
                new {p = "*?", i = ""},
                new {p = "??*", i = "a"},
                new {p = "*abc", i = "abX"},
                new {p = "*abc*", i = "Xbc"},
                new {p = "*a*bc*", i = "ac"}
            };

            foreach (var test in tests)
            {
                var matcher = new WildcardMatcher(test.p);
                Check.That(matcher.IsMatch(test.i)).IsEqualTo(0.0);
            }
        }

        [Fact]
        public void WildcardMatcher_GetName()
        {
            // Assign
            var matcher = new WildcardMatcher("x");

            // Act
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("WildcardMatcher");
        }

        [Fact]
        public void WildcardMatcher_GetPatterns()
        {
            // Assign
            var matcher = new WildcardMatcher("x");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("x");
        }

        [Fact]
        public void WildcardMatcher_IsMatch_RejectOnMatch()
        {
            // Assign
            var matcher = new WildcardMatcher(MatchBehaviour.RejectOnMatch, "m");

            // Act
            double result = matcher.IsMatch("m");

            Check.That(result).IsEqualTo(0.0);
        }
    }
}
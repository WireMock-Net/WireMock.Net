using NUnit.Framework;
using WireMock.Matchers;

namespace WireMock.Net.Tests
{
    [TestFixture]
    public class WildcardMatcherTest
    {
        [Test]
        public void WildcardMatcher_patterns_positive()
        {
            var tests = new[]
            {
                new { p = "*", i = "" },
                new { p = "?", i = " " },
                new { p = "*", i = "a" },
                new { p = "*", i = "ab" },
                new { p = "?", i = "a" },
                new { p = "*?", i = "abc" },
                new { p = "?*", i = "abc" },
                new { p = "abc", i = "abc" },
                new { p = "abc*", i = "abc" },
                new { p = "abc*", i = "abcd" },
                new { p = "*abc*", i = "abc" },
                new { p = "*a*bc*", i = "abc" },
                new { p = "*a*b?", i = "aXXXbc" }
            };
            foreach (var test in tests)
            {
                var matcher = new WildcardMatcher(test.p);
                Assert.IsTrue(matcher.IsMatch(test.i), "p = " + test.p + ", i = " + test.i);
            }
        }

        [Test]
        public void WildcardMatcher_patterns_negative()
        {
            var tests = new[]
            {
                new { p = "*a", i = ""},
                new { p = "a*", i = ""},
                new { p = "?", i = ""},
                new { p = "*b*", i = "a"},
                new { p = "b*a", i = "ab"},
                new { p = "??", i = "a"},
                new { p = "*?", i = ""},
                new { p = "??*", i = "a"},
                new { p = "*abc", i = "abX"},
                new { p = "*abc*", i = "Xbc"},
                new { p = "*a*bc*", i = "ac"}
            };
            foreach (var test in tests)
            {
                var matcher = new WildcardMatcher(test.p);
                Assert.IsFalse(matcher.IsMatch(test.i), "p = " + test.p + ", i = " + test.i);
            }
        }
    }
}
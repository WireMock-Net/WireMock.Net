using System.Diagnostics.CodeAnalysis;
using NFluent;
using NUnit.Framework;

[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules", 
        "SA1600:ElementsMustBeDocumented", 
        Justification = "Reviewed. Suppression is OK here, as it's a tests class.")]
[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules", 
        "SA1633:FileMustHaveHeader", 
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]
// ReSharper disable InconsistentNaming
namespace WireMock.Net.Tests
{
    [TestFixture]
    public class WildcardPatternMatcherTests
    {
        [Test]
        public void Should_evaluate_patterns()
        {
            // Positive Tests
            Check.That(WildcardPatternMatcher.MatchWildcardString("*", string.Empty)).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("?", " ")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*", "a")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*", "ab")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("?", "a")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*?", "abc")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("?*", "abc")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*abc", "abc")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*abc*", "abc")).IsTrue();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*a*bc*", "aXXXbc")).IsTrue();

            // Negative Tests
            Check.That(WildcardPatternMatcher.MatchWildcardString("*a", string.Empty)).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("a*", string.Empty)).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("?", string.Empty)).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*b*", "a")).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("b*a", "ab")).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("??", "a")).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*?", string.Empty)).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("??*", "a")).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*abc", "abX")).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*abc*", "Xbc")).IsFalse();
            Check.That(WildcardPatternMatcher.MatchWildcardString("*a*bc*", "ac")).IsFalse();
        }
    }
}

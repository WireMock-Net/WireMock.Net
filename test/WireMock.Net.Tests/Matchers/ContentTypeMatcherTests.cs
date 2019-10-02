using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class ContentTypeMatcherTests
    {
        [Theory]
        [InlineData("application/json")]
        [InlineData("application/json; charset=ascii")]
        [InlineData("application/json; charset=utf-8")]
        [InlineData("application/json; charset=UTF-8")]
        public void ContentTypeMatcher_IsMatchWithIgnoreCaseFalse_Positive(string contentType)
        {
            var matcher = new ContentTypeMatcher("application/json");
            Check.That(matcher.IsMatch(contentType)).IsEqualTo(1.0d);
        }

        [Theory]
        [InlineData("application/json")]
        [InlineData("application/JSON")]
        [InlineData("application/json; CharSet=ascii")]
        [InlineData("application/json; charset=utf-8")]
        [InlineData("application/json; charset=UTF-8")]
        public void ContentTypeMatcher_IsMatchWithIgnoreCaseTrue_Positive(string contentType)
        {
            var matcher = new ContentTypeMatcher("application/json", true);
            Check.That(matcher.IsMatch(contentType)).IsEqualTo(1.0d);
        }

        [Fact]
        public void ContentTypeMatcher_GetName()
        {
            // Assign
            var matcher = new ContentTypeMatcher("x");

            // Act
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("ContentTypeMatcher");
        }

        [Fact]
        public void ContentTypeMatcher_GetPatterns()
        {
            // Assign
            var matcher = new ContentTypeMatcher("x");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("x");
        }
    }
}
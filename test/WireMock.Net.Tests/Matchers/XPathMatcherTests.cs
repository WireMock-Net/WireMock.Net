using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class XPathMatcherTests
    {
        [Fact]
        public void XPathMatcher_GetName()
        {
            // Assign
            var matcher = new XPathMatcher("X");

            // Act
            string name = matcher.Name;

            // Assert
            Check.That(name).Equals("XPathMatcher");
        }

        [Fact]
        public void XPathMatcher_GetPatterns()
        {
            // Assign
            var matcher = new XPathMatcher("X");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("X");
        }

        [Fact]
        public void XPathMatcher_IsMatch_AcceptOnMatch()
        {
            // Assign
            string xml = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                    </todo-list>";
            var matcher = new XPathMatcher("/todo-list[count(todo-item) = 1]");

            // Act
            double result = matcher.IsMatch(xml);

            // Assert
            Check.That(result).IsEqualTo(1.0);
        }

        [Fact]
        public void XPathMatcher_IsMatch_RejectOnMatch()
        {
            // Assign
            string xml = @"
                    <todo-list>
                        <todo-item id='a1'>abc</todo-item>
                    </todo-list>";
            var matcher = new XPathMatcher(MatchBehaviour.RejectOnMatch, "/todo-list[count(todo-item) = 1]");

            // Act
            double result = matcher.IsMatch(xml);

            // Assert
            Check.That(result).IsEqualTo(0.0);
        }
    }
}
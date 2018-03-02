using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class ExactObjectMatcherTests
    {
        [Fact]
        public void ExactObjectMatcher_GetName()
        {
            // Assign
            object obj = 1;

            // Act
            var matcher = new ExactObjectMatcher(obj);
            string name = matcher.GetName();

            // Assert
            Check.That(name).Equals("ExactObjectMatcher");
        }
    }
}
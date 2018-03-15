using NFluent;
using WireMock.Matchers;
using Xunit;

namespace WireMock.Net.Tests.Matchers
{
    public class SimMetricsMatcherTests
    {
        [Fact]
        public void SimMetricsMatcher_GetName()
        {
            // Assign
            var matcher = new SimMetricsMatcher("X");

            // Act
            string name = matcher.GetName();

            // Assert
            Check.That(name).Equals("SimMetricsMatcher.Levenstein");
        }

        [Fact]
        public void SimMetricsMatcher_GetPatterns()
        {
            // Assign
            var matcher = new SimMetricsMatcher("X");

            // Act
            string[] patterns = matcher.GetPatterns();

            // Assert
            Check.That(patterns).ContainsExactly("X");
        }

        [Fact]
        public void SimMetricsMatcher_IsMatch_1()
        {
            // Assign
            var matcher = new SimMetricsMatcher("The cat walks in the street.");

            // Act
            double result = matcher.IsMatch("The car drives in the street.");

            // Assert
            Check.That(result).IsStrictlyLessThan(1.0).And.IsStrictlyGreaterThan(0.5);
        }

        [Fact]
        public void SimMetricsMatcher_IsMatch_2()
        {
            // Assign
            var matcher = new SimMetricsMatcher("The cat walks in the street.");

            // Act
            double result = matcher.IsMatch("Hello");

            // Assert
            Check.That(result).IsStrictlyLessThan(0.1).And.IsStrictlyGreaterThan(0.05);
        }
    }
}
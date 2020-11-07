using FluentAssertions;
using NFluent;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util
{
    public class PortUtilsTests
    {
        [Fact]
        public void PortUtils_TryExtract_InvalidUrl_Returns_False()
        {
            // Assign
            string url = "test";

            // Act
            bool result = PortUtils.TryExtract(url, out bool isHttps, out string proto, out string host, out int port);

            // Assert
            result.Should().BeFalse();
            isHttps.Should().BeFalse();
            proto.Should().BeNull();
            host.Should().BeNull();
            port.Should().Be(default(int));
        }

        [Fact]
        public void PortUtils_TryExtract_UrlIsMissingPort_Returns_False()
        {
            // Assign
            string url = "http://0.0.0.0";

            // Act
            bool result = PortUtils.TryExtract(url, out bool isHttps, out string proto, out string host, out int port);

            // Assert
            result.Should().BeFalse();
            isHttps.Should().BeFalse();
            proto.Should().BeNull();
            host.Should().BeNull();
            port.Should().Be(default(int));
        }

        [Fact]
        public void PortUtils_TryExtract_Http_Returns_True()
        {
            // Assign
            string url = "http://wiremock.net:1234";

            // Act
            bool result = PortUtils.TryExtract(url, out bool isHttps, out string proto, out string host, out int port);

            // Assert
            result.Should().BeTrue();
            isHttps.Should().BeFalse();
            proto.Should().Be("http");
            host.Should().Be("wiremock.net");
            port.Should().Be(1234);
        }

        [Fact]
        public void PortUtils_TryExtract_Https_Returns_True()
        {
            // Assign
            string url = "https://wiremock.net:5000";

            // Act
            bool result = PortUtils.TryExtract(url, out bool isHttps, out string proto, out string host, out int port);

            // Assert
            result.Should().BeTrue();
            isHttps.Should().BeTrue();
            proto.Should().Be("https");
            host.Should().Be("wiremock.net");
            port.Should().Be(5000);
        }

        [Fact]
        public void PortUtils_TryExtract_Https0_0_0_0_Returns_True()
        {
            // Assign
            string url = "https://0.0.0.0:5000";

            // Act
            bool result = PortUtils.TryExtract(url, out bool isHttps, out string proto, out string host, out int port);

            // Assert
            Check.That(result).IsTrue();
            Check.That(proto).IsEqualTo("https");
            Check.That(host).IsEqualTo("0.0.0.0");
            Check.That(port).IsEqualTo(5000);
        }
    }
}
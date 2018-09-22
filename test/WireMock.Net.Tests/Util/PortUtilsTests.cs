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
            bool result = PortUtils.TryExtract(url, out string proto, out string host, out int port);

            // Assert
            Check.That(result).IsFalse();
            Check.That(proto).IsNull();
            Check.That(host).IsNull();
            Check.That(port).IsEqualTo(default(int));
        }

        [Fact]
        public void PortUtils_TryExtract_UrlIsMissingPort_Returns_False()
        {
            // Assign
            string url = "http://0.0.0.0";

            // Act
            bool result = PortUtils.TryExtract(url, out string proto, out string host, out int port);

            // Assert
            Check.That(result).IsFalse();
            Check.That(proto).IsNull();
            Check.That(host).IsNull();
            Check.That(port).IsEqualTo(default(int));
        }

        [Fact]
        public void PortUtils_TryExtract_ValidUrl1_Returns_True()
        {
            // Assign
            string url = "https://wiremock.net:5000";

            // Act
            bool result = PortUtils.TryExtract(url, out string proto, out string host, out int port);

            // Assert
            Check.That(result).IsTrue();
            Check.That(proto).IsEqualTo("https");
            Check.That(host).IsEqualTo("wiremock.net");
            Check.That(port).IsEqualTo(5000);
        }

        [Fact]
        public void PortUtils_TryExtract_ValidUrl2_Returns_True()
        {
            // Assign
            string url = "https://0.0.0.0:5000";

            // Act
            bool result = PortUtils.TryExtract(url, out string proto, out string host, out int port);

            // Assert
            Check.That(result).IsTrue();
            Check.That(proto).IsEqualTo("https");
            Check.That(host).IsEqualTo("0.0.0.0");
            Check.That(port).IsEqualTo(5000);
        }
    }
}
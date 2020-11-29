using System.Text;
using FluentAssertions;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util
{
    public class BytesEncodingUtilsTests
    {
        [Fact]
        public void TryGetEncoding_InvalidBytes()
        {
            var result = BytesEncodingUtils.TryGetEncoding(new byte[] {0, 1}, out Encoding encoding);

            // Assert
            result.Should().BeFalse();
            encoding.Should().BeNull();
        }
    }
}
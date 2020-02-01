using NFluent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util
{
    public class PathUtilsTests
    {
        [Theory]
        [InlineData(@"subdirectory/MyXmlResponse.xml")]
        [InlineData(@"subdirectory\MyXmlResponse.xml")]
        [InlineData(@"/subdirectory/MyXmlResponse.xml")]
        [InlineData(@"\subdirectory\MyXmlResponse.xml")]
        public void PathUtils_CleanPath_CheckSlashes(string path)
        {
            // Act
            var cleanPath = PathUtils.CleanPath(path);

            // Assert
            Check.That(cleanPath).IsNotNull();
            Check.That(cleanPath).IsNotEmpty();
            Check.That(cleanPath).Equals("subdirectory" + Path.DirectorySeparatorChar + "MyXmlResponse.xml");
            Check.That(cleanPath).IsEqualTo("subdirectory" + Path.DirectorySeparatorChar + "MyXmlResponse.xml");
        }
    }
}
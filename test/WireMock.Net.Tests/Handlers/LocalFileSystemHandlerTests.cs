using System;
using System.IO;
using NFluent;
using WireMock.Handlers;
using Xunit;

namespace WireMock.Net.Tests.Handlers
{
    public class LocalFileSystemHandlerTests
    {
        private LocalFileSystemHandler sut = new LocalFileSystemHandler();

        [Fact]
        public void LocalFileSystemHandler_GetMappingFolder()
        {
            // Act
            string result = sut.GetMappingFolder();

            // Assert
            Check.That(result).EndsWith(Path.Combine("__admin", "mappings"));
        }

        [Fact]
        public void LocalFileSystemHandler_CreateFolder_Throws()
        {
            // Act
            Check.ThatCode(() => sut.CreateFolder(null)).Throws<ArgumentNullException>();
        }

        [Fact]
        public void LocalFileSystemHandler_WriteMappingFile_Throws()
        {
            // Act
            Check.ThatCode(() => sut.WriteMappingFile(null, null)).Throws<ArgumentNullException>();
        }
    }
}
using NFluent;
using System;
using System.IO;
using WireMock.Handlers;
using Xunit;

namespace WireMock.Net.Tests.Handlers
{
    public class LocalFileSystemHandlerTests
    {
        private readonly LocalFileSystemHandler _sut = new LocalFileSystemHandler();

        [Fact]
        public void LocalFileSystemHandler_GetMappingFolder()
        {
            // Act
            string result = _sut.GetMappingFolder();

            // Assert
            Check.That(result).EndsWith(Path.Combine("__admin", "mappings"));
        }

        [Fact]
        public void LocalFileSystemHandler_CreateFolder_Throws()
        {
            // Act
            Check.ThatCode(() => _sut.CreateFolder(null)).Throws<ArgumentNullException>();
        }

        [Fact]
        public void LocalFileSystemHandler_WriteMappingFile_Throws()
        {
            // Act
            Check.ThatCode(() => _sut.WriteMappingFile(null, null)).Throws<ArgumentNullException>();
        }

        [Fact]
        public void LocalFileSystemHandler_ReadResponseBodyAsFile_Throws()
        {
            // Act
            Check.ThatCode(() => _sut.ReadResponseBodyAsFile(null)).Throws<ArgumentNullException>();
        }

        [Fact]
        public void LocalFileSystemHandler_ReadFile_Throws()
        {
            // Act
            Check.ThatCode(() => _sut.ReadFile(null)).Throws<ArgumentNullException>();
        }

        [Fact]
        public void LocalFileSystemHandler_WriteFile_Throws()
        {
            // Act
            Check.ThatCode(() => _sut.WriteFile(null, null)).Throws<ArgumentNullException>();
        }

        [Fact]
        public void LocalFileSystemHandler_DeleteFile_Throws()
        {
            // Act
            Check.ThatCode(() => _sut.DeleteFile(null)).Throws<ArgumentNullException>();
        }
    }
}
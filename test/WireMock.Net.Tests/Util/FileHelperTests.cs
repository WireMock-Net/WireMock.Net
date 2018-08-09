using System;
using System.IO;
using Moq;
using NFluent;
using WireMock.Handlers;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util
{
    public class FileHelperTests
    {
        [Fact]
        public void FileHelper_ReadAllTextWithRetryAndDelay()
        {
            // Assign
            var _staticMappingHandlerMock = new Mock<IStaticMappingHandler>();
            _staticMappingHandlerMock.Setup(m => m.ReadMappingFile).Returns((string path) => "text");

            // Act
            string result = FileHelper.ReadAllTextWithRetryAndDelay(_staticMappingHandlerMock.Object, @"c:\temp");

            // Assert
            Check.That(result).Equals("text");

            // Verify
            _staticMappingHandlerMock.Verify(m => m.ReadMappingFile, Times.Once);
        }

        [Fact]
        public void FileHelper_ReadAllTextWithRetryAndDelay_Throws()
        {
            // Assign
            var _staticMappingHandlerMock = new Mock<IStaticMappingHandler>();
            _staticMappingHandlerMock.Setup(m => m.ReadMappingFile).Throws<NotSupportedException>();

            // Act
            Check.ThatCode(() => FileHelper.ReadAllTextWithRetryAndDelay(_staticMappingHandlerMock.Object, @"c:\temp")).Throws<IOException>();

            // Verify
            _staticMappingHandlerMock.Verify(m => m.ReadMappingFile, Times.Exactly(3));
        }
    }
}

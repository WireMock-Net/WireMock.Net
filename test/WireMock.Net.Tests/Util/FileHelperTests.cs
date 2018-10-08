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
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.ReadMappingFile(It.IsAny<string>())).Returns("text");

            // Act
            string result = FileHelper.ReadAllTextWithRetryAndDelay(staticMappingHandlerMock.Object, @"c:\temp");

            // Assert
            Check.That(result).Equals("text");

            // Verify
            staticMappingHandlerMock.Verify(m => m.ReadMappingFile(@"c:\temp"), Times.Once);
        }

        [Fact]
        public void FileHelper_ReadAllTextWithRetryAndDelay_Throws()
        {
            // Assign
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.ReadMappingFile(It.IsAny<string>())).Throws<NotSupportedException>();

            // Act
            Check.ThatCode(() => FileHelper.ReadAllTextWithRetryAndDelay(staticMappingHandlerMock.Object, @"c:\temp")).Throws<IOException>();

            // Verify
            staticMappingHandlerMock.Verify(m => m.ReadMappingFile(@"c:\temp"), Times.Exactly(3));
        }
    }
}

using Moq;
using NFluent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WireMock.Handlers;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class WireMockServerAdminFilesTests
    {
        private readonly HttpClient _client = new HttpClient();

        [Fact]
        public async Task WireMockServer_Admin_Files_Post_Ascii()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.GetMappingFolder()).Returns("__admin/mappings");
            filesystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);
            filesystemHandlerMock.Setup(fs => fs.WriteFile(It.IsAny<string>(), It.IsAny<byte[]>()));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            multipartFormDataContent.Add(new StreamContent(new MemoryStream(Encoding.ASCII.GetBytes("Here's a string."))));

            // Act
            var httpResponseMessage = await _client.PostAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt", multipartFormDataContent).ConfigureAwait(false);

            // Assert
            Check.That(httpResponseMessage.StatusCode).Equals(HttpStatusCode.OK);
            Check.That(server.LogEntries.Count().Equals(1));

            // Verify
            filesystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.FolderExists(It.IsAny<string>()), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task WireMockServer_Admin_Files_Post_MappingFolderDoesNotExistsButWillBeCreated()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.GetMappingFolder()).Returns("x");
            filesystemHandlerMock.Setup(fs => fs.CreateFolder(It.IsAny<string>()));
            filesystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(false);
            filesystemHandlerMock.Setup(fs => fs.WriteFile(It.IsAny<string>(), It.IsAny<byte[]>()));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            multipartFormDataContent.Add(new StreamContent(new MemoryStream(Encoding.ASCII.GetBytes("Here's a string."))));

            // Act
            var httpResponseMessage = await _client.PostAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt", multipartFormDataContent).ConfigureAwait(false);

            // Assert
            Check.That(httpResponseMessage.StatusCode).Equals(HttpStatusCode.OK);

            // Verify
            filesystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.FolderExists(It.IsAny<string>()), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.CreateFolder(It.Is<string>(p => p == "x")), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task WireMockServer_Admin_Files_GetAscii()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(Encoding.ASCII.GetBytes("Here's a string."));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            multipartFormDataContent.Add(new StreamContent(new MemoryStream()));

            // Act
            var httpResponseMessageGet = await _client.GetAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt").ConfigureAwait(false);

            // Assert
            Check.That(httpResponseMessageGet.StatusCode).Equals(HttpStatusCode.OK);

            Check.That(httpResponseMessageGet.Content.ReadAsStringAsync().Result).Equals("Here's a string.");

            Check.That(server.LogEntries.Count().Equals(2));

            // Verify
            filesystemHandlerMock.Verify(fs => fs.ReadFile(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task WireMockServer_Admin_Files_GetUTF16()
        {
            // Arrange
            byte[] symbol = Encoding.UTF32.GetBytes(char.ConvertFromUtf32(0x1D161));
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(symbol);

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            multipartFormDataContent.Add(new StreamContent(new MemoryStream()));

            // Act
            var httpResponseMessageGet = await _client.GetAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.bin").ConfigureAwait(false);

            // Assert
            Check.That(httpResponseMessageGet.StatusCode).Equals(HttpStatusCode.OK);
            Check.That(httpResponseMessageGet.Content.ReadAsByteArrayAsync().Result).Equals(symbol);
            Check.That(server.LogEntries.Count().Equals(2));

            // Verify
            filesystemHandlerMock.Verify(fs => fs.ReadFile(It.Is<string>(p => p == "filename.bin")), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.bin")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task WireMockServer_Admin_Files_Head()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            // Act
            var requestUri = "http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, requestUri);
            var httpResponseMessage = await _client.SendAsync(httpRequestMessage).ConfigureAwait(false);

            // Assert
            Check.That(httpResponseMessage.StatusCode).Equals(HttpStatusCode.NoContent);
            Check.That(server.LogEntries.Count().Equals(1));

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.IsAny<string>()), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task WireMockServer_Admin_Files_Head_FileDoesNotExistsReturns404()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            // Act
            var requestUri = "http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, requestUri);
            var httpResponseMessage = await _client.SendAsync(httpRequestMessage).ConfigureAwait(false);

            // Assert
            Check.That(httpResponseMessage.StatusCode).Equals(HttpStatusCode.NotFound);
            Check.That(server.LogEntries.Count().Equals(1));

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.IsAny<string>()), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();
        }
    }
}
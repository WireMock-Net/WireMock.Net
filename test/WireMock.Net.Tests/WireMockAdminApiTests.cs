using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NFluent;
using RestEase;
using WireMock.Client;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Models.Mappings;
using WireMock.Models.Settings;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class WireMockAdminApiTests
    {
        [Fact]
        public async Task IWireMockAdminApi_GetSettingsAsync()
        {
            // Arrange
            var server = WireMockServer.StartWithAdminInterface();
            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var settings = await api.GetSettingsAsync();
            Check.That(settings).IsNotNull();
        }

        [Fact]
        public async Task IWireMockAdminApi_PostSettingsAsync()
        {
            // Arrange
            var server = WireMockServer.StartWithAdminInterface();
            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var settings = new SettingsModel();
            var status = await api.PostSettingsAsync(settings);
            Check.That(status.Status).Equals("Settings updated");
        }

        [Fact]
        public async Task IWireMockAdminApi_PutSettingsAsync()
        {
            // Arrange
            var server = WireMockServer.StartWithAdminInterface();
            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var settings = new SettingsModel();
            var status = await api.PutSettingsAsync(settings);
            Check.That(status.Status).Equals("Settings updated");
        }

        // https://github.com/WireMock-Net/WireMock.Net/issues/325
        [Fact]
        public async Task IWireMockAdminApi_PutMappingAsync()
        {
            // Arrange
            var server = WireMockServer.StartWithAdminInterface();
            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var model = new MappingModel
            {
                Request = new RequestModel { Path = "/1" },
                Response = new ResponseModel { Body = "txt", StatusCode = 200 },
                Priority = 500,
                Title = "test"
            };
            var result = await api.PutMappingAsync(new Guid("a0000000-0000-0000-0000-000000000000"), model);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).Equals("Mapping added or updated");
            Check.That(result.Guid).IsNotNull();

            var mapping = server.Mappings.Single(m => m.Priority == 500);
            Check.That(mapping).IsNotNull();
            Check.That(mapping.Title).Equals("test");

            server.Stop();
        }

        [Fact]
        public async Task IWireMockAdminApi_PostMappingAsync()
        {
            // Arrange
            var server = WireMockServer.StartWithAdminInterface();
            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var model = new MappingModel
            {
                Request = new RequestModel { Path = "/1" },
                Response = new ResponseModel { Body = "txt", StatusCode = 200 },
                Priority = 500,
                Title = "test"
            };
            var result = await api.PostMappingAsync(model);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsNotNull();
            Check.That(result.Guid).IsNotNull();

            var mapping = server.Mappings.Single(m => m.Priority == 500);
            Check.That(mapping).IsNotNull();
            Check.That(mapping.Title).Equals("test");

            server.Stop();
        }

        [Fact]
        public async Task IWireMockAdminApi_PostMappingsAsync()
        {
            // Arrange
            var server = WireMockServer.StartWithAdminInterface();
            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var model1 = new MappingModel
            {
                Request = new RequestModel { Path = "/1" },
                Response = new ResponseModel { Body = "txt 1" },
                Title = "test 1"
            };
            var model2 = new MappingModel
            {
                Request = new RequestModel { Path = "/2" },
                Response = new ResponseModel { Body = "txt 2" },
                Title = "test 2"
            };
            var result = await api.PostMappingsAsync(new[] { model1, model2 });

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsNotNull();
            Check.That(result.Guid).IsNull();
            Check.That(server.Mappings.Where(m => !m.IsAdminInterface)).HasSize(2);

            server.Stop();
        }

        [Fact]
        public async Task IWireMockAdminApi_FindRequestsAsync()
        {
            // Arrange
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true,
                Logger = new WireMockNullLogger()
            });
            var serverUrl = "http://localhost:" + server.Ports[0];
            await new HttpClient().GetAsync(serverUrl + "/foo");
            var api = RestClient.For<IWireMockAdminApi>(serverUrl);

            // Act
            var requests = await api.FindRequestsAsync(new RequestModel { Methods = new[] { "GET" } });

            // Assert
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("GET");
            Check.That(requestLogged.Request.Body).IsNull();
            Check.That(requestLogged.Request.Path).IsEqualTo("/foo");
        }

        [Fact]
        public async Task IWireMockAdminApi_GetRequestsAsync()
        {
            // Arrange
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true,
                Logger = new WireMockNullLogger()
            });
            var serverUrl = "http://localhost:" + server.Ports[0];
            await new HttpClient().GetAsync(serverUrl + "/foo");
            var api = RestClient.For<IWireMockAdminApi>(serverUrl);

            // Act
            var requests = await api.GetRequestsAsync();

            // Assert
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("GET");
            Check.That(requestLogged.Request.Body).IsNull();
            Check.That(requestLogged.Request.Path).IsEqualTo("/foo");
        }

        [Fact]
        public async Task IWireMockAdminApi_GetRequestsAsync_JsonApi()
        {
            // Arrange
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true,
                Logger = new WireMockNullLogger()
            });
            string serverUrl = server.Urls[0];
            string data = "{\"data\":[{\"type\":\"program\",\"attributes\":{\"alias\":\"T000001\",\"title\":\"Title Group Entity\"}}]}";
            string jsonApiAcceptHeader = "application/vnd.api+json";
            string jsonApiContentType = "application/vnd.api+json";

            var request = new HttpRequestMessage(HttpMethod.Post, serverUrl);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonApiAcceptHeader));
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonApiContentType);

            var response = await new HttpClient().SendAsync(request);
            Check.That(response).IsNotNull();

            var api = RestClient.For<IWireMockAdminApi>(serverUrl);

            // Act
            var requests = await api.GetRequestsAsync();

            // Assert
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("POST");
            Check.That(requestLogged.Request.Body).IsNotNull();
            Check.That(requestLogged.Request.Body).Contains("T000001");
        }

        [Fact]
        public async Task IWireMockAdminApi_GetRequestsAsync_Json()
        {
            // Arrange
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true,
                Logger = new WireMockNullLogger()
            });
            string serverUrl = server.Urls[0];
            string data = "{\"alias\": \"T000001\"}";
            string jsonAcceptHeader = "application/json";
            string jsonApiContentType = "application/json";

            var request = new HttpRequestMessage(HttpMethod.Post, serverUrl);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(jsonAcceptHeader));
            request.Content = new StringContent(data);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(jsonApiContentType);
            var response = await new HttpClient().SendAsync(request);
            Check.That(response).IsNotNull();

            var api = RestClient.For<IWireMockAdminApi>(serverUrl);

            // Act
            var requests = await api.GetRequestsAsync();

            // Assert
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("POST");
            Check.That(requestLogged.Request.Body).IsNotNull();
            Check.That(requestLogged.Request.Body).Contains("T000001");
        }

        [Fact]
        public async Task IWireMockAdminApi_PostFileAsync_Ascii()
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

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var request = await api.PostFileAsync("filename.txt", "abc");

            // Assert
            Check.That(request.Guid).IsNull();
            Check.That(request.Status).Contains("File");

            // Verify
            filesystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.FolderExists(It.IsAny<string>()), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }

        [Fact]
        public async Task IWireMockAdminApi_PutFileAsync_Ascii()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            filesystemHandlerMock.Setup(fs => fs.WriteFile(It.IsAny<string>(), It.IsAny<byte[]>()));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            var request = await api.PutFileAsync("filename.txt", "abc-abc");

            // Assert
            Check.That(request.Guid).IsNull();
            Check.That(request.Status).Contains("File");

            // Verify
            filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }

        [Fact]
        public void IWireMockAdminApi_PutFileAsync_NotFound()
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

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act and Assert
            Check.ThatAsyncCode(() => api.PutFileAsync("filename.txt", "xxx")).Throws<ApiException>();

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }

        [Fact]
        public void IWireMockAdminApi_GetFileAsync_NotFound()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);
            filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(Encoding.ASCII.GetBytes("Here's a string."));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act and Assert
            Check.ThatAsyncCode(() => api.GetFileAsync("filename.txt")).Throws<ApiException>();

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }

        [Fact]
        public async Task IWireMockAdminApi_GetFileAsync_Found()
        {
            // Arrange
            string data = "Here's a string.";
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(Encoding.ASCII.GetBytes(data));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            string file = await api.GetFileAsync("filename.txt");

            // Assert
            Check.That(file).Equals(data);

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.ReadFile(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }

        [Fact]
        public async Task IWireMockAdminApi_DeleteFileAsync_Ok()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
            filesystemHandlerMock.Setup(fs => fs.DeleteFile(It.IsAny<string>()));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act
            await api.DeleteFileAsync("filename.txt");

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.Verify(fs => fs.DeleteFile(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }

        [Fact]
        public void IWireMockAdminApi_DeleteFileAsync_NotFound()
        {
            // Arrange
            var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);
            filesystemHandlerMock.Setup(fs => fs.DeleteFile(It.IsAny<string>()));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                UseSSL = false,
                StartAdminInterface = true,
                FileSystemHandler = filesystemHandlerMock.Object
            });

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act and Assert
            Check.ThatAsyncCode(() => api.DeleteFileAsync("filename.txt")).Throws<ApiException>();

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }

        [Fact]
        public void IWireMockAdminApi_FileExistsAsync_NotFound()
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

            var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

            // Act and Assert
            Check.ThatAsyncCode(() => api.FileExistsAsync("filename.txt")).Throws<ApiException>();

            // Verify
            filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
            filesystemHandlerMock.VerifyNoOtherCalls();

            server.Stop();
        }
    }
}
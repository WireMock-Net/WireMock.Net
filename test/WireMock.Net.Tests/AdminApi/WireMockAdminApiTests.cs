// Copyright © WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NFluent;
using RestEase;
using VerifyTests;
using VerifyXunit;
using WireMock.Admin.Mappings;
using WireMock.Admin.Settings;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.AdminApi;

[UsesVerify]
public partial class WireMockAdminApiTests
{
    private static readonly VerifySettings VerifySettings = new();

    private readonly Mock<IGuidUtils> _guidUtilsMock = new();

    public WireMockAdminApiTests()
    {
        var startGuid = 1000;
        _guidUtilsMock.Setup(g => g.NewGuid()).Returns(() => new Guid($"98fae52e-76df-47d9-876f-2ee32e93{startGuid++}"));
    }

    static WireMockAdminApiTests()
    {
        VerifyNewtonsoftJson.Enable(VerifySettings);
    }

    [Fact]
    public async Task IWireMockAdminApi_WaitForHealthAsync_AndCall_GetHealthAsync_OK()
    {
        // Arrange
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var server = WireMockServer.Start(w =>
        {
            w.StartAdminInterface = true;
            w.AdminUsername = adminUsername;
            w.AdminPassword = adminPassword;
        });
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0])
            .WithAuthorization(adminUsername, adminPassword);

        // Act 1
        await api.WaitForHealthAsync().ConfigureAwait(false);

        // Act 2
        var status = await api.GetHealthAsync().ConfigureAwait(false);
        status.Should().Be("Healthy");
    }

    [Fact]
    public async Task IWireMockAdminApi_WaitForHealthAsync_AndCall_GetHealthAsync_ThrowsException()
    {
        // Arrange
        var server = WireMockServer.Start(w =>
        {
            w.StartAdminInterface = true;
            w.AdminUsername = $"username_{Guid.NewGuid()}";
            w.AdminPassword = $"password_{Guid.NewGuid()}";
        });

        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        Func<Task> act = () => api.WaitForHealthAsync(maxRetries: 3);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetSettingsAsync()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var settings = await api.GetSettingsAsync().ConfigureAwait(false);
        Check.That(settings).IsNotNull();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetSettingsAsync_ForDifferentAdminPath()
    {
        // Arrange
        var server = WireMockServer.Start(w =>
        {
            w.StartAdminInterface = true;
            w.AdminPath = "/foo/__admin";
        });
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0] + "/foo");

        // Act
        var settings = await api.GetSettingsAsync().ConfigureAwait(false);

        // Assert
        Check.That(settings).IsNotNull();

        // Cleanup
        server.Stop();
        server.Dispose();
    }

    [Fact]
    public async Task IWireMockAdminApi_PostSettingsAsync()
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var settings = new SettingsModel();
        var status = await api.PostSettingsAsync(settings).ConfigureAwait(false);
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
        var status = await api.PutSettingsAsync(settings).ConfigureAwait(false);
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
        var result = await api.PutMappingAsync(new Guid("a0000000-0000-0000-0000-000000000000"), model).ConfigureAwait(false);

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result.Status).Equals("Mapping added or updated");
        Check.That(result.Guid).IsNotNull();

        var mapping = server.Mappings.Single(m => m.Priority == 500);
        Check.That(mapping).IsNotNull();
        Check.That(mapping.Title).Equals("test");

        server.Stop();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(200, 200)]
    [InlineData("200", "200")]
    public async Task IWireMockAdminApi_PostMappingAsync_WithStatusCode(object statusCode, object expectedStatusCode)
    {
        // Arrange
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var model = new MappingModel
        {
            Request = new RequestModel { Path = "/1" },
            Response = new ResponseModel { Body = "txt", StatusCode = statusCode },
            Priority = 500,
            Title = "test"
        };
        var result = await api.PostMappingAsync(model).ConfigureAwait(false);

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result.Status).IsNotNull();
        Check.That(result.Guid).IsNotNull();

        var mapping = server.Mappings.Single(m => m.Priority == 500);
        Check.That(mapping).IsNotNull();
        Check.That(mapping.Title).Equals("test");

        var response = await mapping.ProvideResponseAsync(new RequestMessage(new UrlDetails("http://localhost/1"), "GET", "")).ConfigureAwait(false);
        Check.That(response.Message.StatusCode).Equals(expectedStatusCode);

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
        var result = await api.PostMappingsAsync(new[] { model1, model2 }).ConfigureAwait(false);

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result.Status).IsNotNull();
        Check.That(result.Guid).IsNull();
        Check.That(server.Mappings.Where(m => !m.IsAdminInterface)).HasSize(2);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_PostMappingsAsync_WithDuplicateGuids_Should_Return_400()
    {
        // Arrange
        var guid = Guid.Parse("1b731398-4a5b-457f-a6e3-d65e541c428f");
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var model1WithGuid = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel { Path = "/1g" },
            Response = new ResponseModel { Body = "txt 1g" },
            Title = "test 1g"
        };
        var model2WithGuid = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel { Path = "/2g" },
            Response = new ResponseModel { Body = "txt 2g" },
            Title = "test 2g"
        };
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

        var models = new[]
        {
                model1WithGuid,
                model2WithGuid,
                model1,
                model2
            };

        var sutMethod = async () => await api.PostMappingsAsync(models);
        var exceptionAssertions = await sutMethod.Should().ThrowAsync<ApiException>();
        exceptionAssertions.Which.Content.Should().Be(@"{""Status"":""The following Guids are duplicate : '1b731398-4a5b-457f-a6e3-d65e541c428f' (Parameter 'mappingModels')""}");

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
        server
            .Given(Request.Create().WithPath("/foo").UsingGet())
            .RespondWith(Response.Create());

        var serverUrl = "http://localhost:" + server.Ports[0];
        await new HttpClient().GetAsync(serverUrl + "/foo").ConfigureAwait(false);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var requests = await api.FindRequestsAsync(new RequestModel { Methods = new[] { "GET" } }).ConfigureAwait(false);

        // Assert
        requests.Should().HaveCount(1);
        var requestLogged = requests.First();
        requestLogged.Request.Method.Should().Be("GET");
        requestLogged.Request.Body.Should().BeNull();
        requestLogged.Request.Path.Should().Be("/foo");
    }

    [Fact]
    public async Task IWireMockAdminApi_FindRequestsByMappingGuidAsync_Found()
    {
        // Arrange
        var mappingGuid = Guid.NewGuid();
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        server
            .Given(Request.Create().WithPath("/foo").UsingGet())
            .WithGuid(mappingGuid)
            .RespondWith(Response.Create());

        var serverUrl = "http://localhost:" + server.Ports[0];
        using var client = new HttpClient();
        await client.GetAsync(serverUrl + "/foo").ConfigureAwait(false);
        await client.GetAsync(serverUrl + "/foo?bar=baz").ConfigureAwait(false);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var logEntryModels = await api.FindRequestsByMappingGuidAsync(mappingGuid).ConfigureAwait(false);

        // Assert
        logEntryModels.Should().HaveCount(2);
        logEntryModels[0].Should().NotBeNull();
        logEntryModels[0]!.Request.Method.Should().Be("GET");
        logEntryModels[0]!.Request.Body.Should().BeNull();
        logEntryModels[0]!.Request.Path.Should().Be("/foo");
        logEntryModels[0]!.Request.Query.Should().BeNullOrEmpty();
        logEntryModels[1].Should().NotBeNull();
        logEntryModels[1]!.Request.Method.Should().Be("GET");
        logEntryModels[1]!.Request.Body.Should().BeNull();
        logEntryModels[1]!.Request.Path.Should().Be("/foo");
        logEntryModels[1]!.Request.Query.Should().BeEquivalentTo(new Dictionary<string, WireMockList<string>>
        {
            {"bar", new WireMockList<string>("baz")}
        });
    }

    [Fact]
    public async Task IWireMockAdminApi_FindRequestsByMappingGuidAsync_NotFound()
    {
        // Arrange
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });
        server
            .Given(Request.Create().WithPath("/foo").UsingGet())
            .WithGuid(Guid.NewGuid())
            .RespondWith(Response.Create());

        var serverUrl = "http://localhost:" + server.Ports[0];
        await new HttpClient().GetAsync(serverUrl + "/foo").ConfigureAwait(false);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var logEntryModels = await api.FindRequestsByMappingGuidAsync(Guid.NewGuid()).ConfigureAwait(false);

        // Assert
        logEntryModels.Should().BeEmpty();
    }

    [Fact]
    public async Task IWireMockAdminApi_FindRequestsByMappingGuidAsync_Invalid_ShouldReturnBadRequest()
    {
        // Arrange
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });

        // Act
        var result = await server.CreateClient().GetAsync("/__admin/requests/find?mappingGuid=x");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        await new HttpClient().GetAsync(serverUrl + "/foo").ConfigureAwait(false);
        var api = RestClient.For<IWireMockAdminApi>(serverUrl);

        // Act
        var requests = await api.GetRequestsAsync().ConfigureAwait(false);

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
        var requests = await api.GetRequestsAsync().ConfigureAwait(false);

        // Assert
        Check.That(requests).HasSize(1);
        var requestLogged = requests.First();
        Check.That(requestLogged.Request.Method).IsEqualTo("POST");
        Check.That(requestLogged.Request.Body).IsNotNull();
        Check.That(requestLogged.Request.Body).Contains("T000001");
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingAsync_WithBodyModelMatcherModel_WithoutMethods_ShouldReturnCorrectMappingModel()
    {
        // Arrange
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var model = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel
            {
                Path = "/1",
                Body = new BodyModel
                {
                    Matcher = new MatcherModel
                    {
                        Name = "RegexMatcher",
                        Pattern = "hello",
                        IgnoreCase = true
                    }
                }
            },
            Response = new ResponseModel { Body = "world" }
        };
        var postMappingResult = await api.PostMappingAsync(model).ConfigureAwait(false);

        // Assert
        postMappingResult.Should().NotBeNull();

        var mapping = server.Mappings.FirstOrDefault(m => m.Guid == guid);
        mapping.Should().NotBeNull();

        var getMappingResult = await api.GetMappingAsync(guid).ConfigureAwait(false);

        await Verifier.Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingAsync_WithProxy_And_ProxyUrlReplaceSettings()
    {
        // Arrange
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var model = new MappingModel
        {
            Guid = guid,
            Request = new RequestModel
            {
                Path = "/1",
                Body = new BodyModel
                {
                    Matcher = new MatcherModel
                    {
                        Name = "RegexMatcher",
                        Pattern = "hello",
                        IgnoreCase = true
                    }
                }
            },
            Response = new ResponseModel
            {
                ProxyUrl = "https://my-proxy.com",
                ProxyUrlReplaceSettings = new ProxyUrlReplaceSettingsModel
                {
                    OldValue = "x",
                    NewValue = "y",
                    IgnoreCase = true
                }
            }
        };
        var postMappingResult = await api.PostMappingAsync(model).ConfigureAwait(false);

        // Assert
        postMappingResult.Should().NotBeNull();

        var mapping = server.Mappings.FirstOrDefault(m => m.Guid == guid);
        mapping.Should().NotBeNull();

        var getMappingResult = await api.GetMappingAsync(guid).ConfigureAwait(false);

        await Verifier.Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
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
        var requests = await api.GetRequestsAsync().ConfigureAwait(false);

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
        var request = await api.PostFileAsync("filename.txt", "abc").ConfigureAwait(false);

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
        var request = await api.PutFileAsync("filename.txt", "abc-abc").ConfigureAwait(false);

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
        string file = await api.GetFileAsync("filename.txt").ConfigureAwait(false);

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
        await api.DeleteFileAsync("filename.txt").ConfigureAwait(false);

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

    [Fact]
    public async Task IWireMockAdminApi_DeleteScenarioUsingDeleteAsync()
    {
        // Arrange
        var name = "x";
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var status = await api.DeleteScenarioAsync(name).ConfigureAwait(false);
        status.Status.Should().Be("No scenario found by name 'x'.");
    }

    [Fact]
    public async Task IWireMockAdminApi_DeleteScenarioUsingPostAsync()
    {
        // Arrange
        var name = "x";
        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        // Act
        var status = await api.ResetScenarioAsync(name).ConfigureAwait(false);
        status.Status.Should().Be("No scenario found by name 'x'.");
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingByGuidAsync()
    {
        // Arrange
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();

        server
            .Given(
                Request.Create()
                    .WithPath("/foo1")
                    .WithParam("p1", "xyz")
                    .UsingGet()
            )
            .WithGuid(guid)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody("1")
            );

        // Act
        var api = RestClient.For<IWireMockAdminApi>(server.Url);
        var getMappingResult = await api.GetMappingAsync(guid).ConfigureAwait(false);

        // Assert
        var mapping = server.Mappings.FirstOrDefault(m => m.Guid == guid);
        mapping.Should().NotBeNull();

        await Verifier.Verify(getMappingResult, VerifySettings).DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingCodeByGuidAsync()
    {
        // Arrange
        var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var server = WireMockServer.StartWithAdminInterface();

        server
            .Given(
                Request.Create()
                    .WithPath("/foo1")
                    .WithParam("p1", "xyz")
                    .UsingGet()
            )
            .WithGuid(guid)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody("1")
            );

        // Act
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        var mappings = await api.GetMappingsAsync().ConfigureAwait(false);
        mappings.Should().HaveCount(1);

        var code = await api.GetMappingCodeAsync(guid).ConfigureAwait(false);

        // Assert
        await Verifier.Verify(code).DontScrubDateTimes().DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingsCode()
    {
        // Arrange
        var guid1 = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
        var guid2 = Guid.Parse("1b731398-4a5b-457f-a6e3-d65e541c428f");
        var guid3 = Guid.Parse("f74fd144-df53-404f-8e35-da22a640bd5f");
        var guid4 = Guid.Parse("4126DEC8-470B-4EFF-93BB-C24F83B8B1FD");
        var guid5 = Guid.Parse("c9929240-7ae8-4a5d-8ed8-0913479f6eeb");
        var server = WireMockServer.StartWithAdminInterface();

        server
            .Given(
                Request.Create()
                    .WithPath("/users/post1")
                    .UsingPost()
                    .WithBody(new JsonMatcher(new
                    {
                        city = "Amsterdam",
                        country = "The Netherlands"
                    }))
            )
            .WithGuid(guid1)
            .RespondWith(Response.Create());

        server
            .Given(
                Request.Create()
                    .WithPath("/users/post2")
                    .UsingPost()
                    .WithBody(new JsonPartialMatcher(new
                    {
                        city = "City",
                        country = "Country"
                    }))
            )
            .WithGuid(guid2)
            .RespondWith(Response.Create().WithBody("Line1\r\nSome \"value\" in Line2"));

        server
            .Given(
                Request.Create()
                    .WithPath("/foo1")
                    .WithParam("p1", "xyz")
                    .UsingGet()
            )
            .WithGuid(guid3)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody("1")
            );

        server
            .Given(
                Request.Create()
                    .WithPath("/foo2")
                    .WithParam("p2", "abc")
                    .WithHeader("h1", "W/\"234f2q3r\"")
                    .UsingPost()
            )
            .WithGuid(guid4)
            .RespondWith(
                Response.Create()
                    .WithStatusCode("201")
                    .WithHeader("hk", "hv")
                    .WithHeader("ETag", "W/\"168d8e\"")
                    .WithBody("2")
            );

        server
            .Given(
                Request.Create()
                    .WithUrl("https://localhost/test")
                    .UsingDelete()
            )
            .WithGuid(guid5)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(HttpStatusCode.AlreadyReported)
                    .WithBodyAsJson(new
                    {
                        @as = 1,
                        b = 1.2,
                        d = true,
                        e = false,
                        f = new[] { 1, 2, 3, 4 },
                        g = new
                        {
                            z1 = 1,
                            z2 = 2,
                            z3 = new[] { "a", "b", "c" },
                            z4 = new[]
                            {
                                new { a = 1, b = 2 },
                                new { a = 2, b = 3 }
                            }
                        },
                        date_field = new DateTime(2023, 05, 08, 11, 20, 19),
                        string_field_with_date = "2021-03-13T21:04:00Z",
                        multiline_text = @"This
is
multiline
text
"
                    })
            );

        // Act
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        var mappings = await api.GetMappingsAsync().ConfigureAwait(false);
        mappings.Should().HaveCount(5);

        var code = await api.GetMappingsCodeAsync().ConfigureAwait(false);

        // Assert
        await Verifier.Verify(code).DontScrubDateTimes().DontScrubGuids();

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiConvert_Yml()
    {
        // Arrange
        var openApiDocument = await File.ReadAllTextAsync(Path.Combine("OpenApiParser", "petstore.yml"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var mappings = await api.OpenApiConvertAsync(openApiDocument).ConfigureAwait(false);

        // Assert
        server.MappingModels.Should().BeEmpty();
        mappings.Should().HaveCount(20);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiConvert_Json()
    {
        // Arrange
        var openApiDocument = await File.ReadAllTextAsync(Path.Combine("OpenApiParser", "petstore-openapi3.json"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var mappings = await api.OpenApiConvertAsync(openApiDocument).ConfigureAwait(false);

        // Assert
        server.MappingModels.Should().BeEmpty();
        mappings.Should().HaveCount(19);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiSave_Json()
    {
        // Arrange
        var openApiDocument = await File.ReadAllTextAsync(Path.Combine("OpenApiParser", "petstore-openapi3.json"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var statusModel = await api.OpenApiSaveAsync(openApiDocument).ConfigureAwait(false);

        // Assert
        statusModel.Status.Should().Be("OpenApi document converted to Mappings");
        server.MappingModels.Should().HaveCount(19);

        server.Stop();
    }

    [Fact]
    public async Task IWireMockAdminApi_OpenApiSave_Yml()
    {
        // Arrange
        var openApiDocument = await File.ReadAllTextAsync(Path.Combine("OpenApiParser", "petstore.yml"));

        var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Url);

        // Act
        var mappings = await api.OpenApiConvertAsync(openApiDocument).ConfigureAwait(false);

        // Assert
        server.MappingModels.Should().BeEmpty();
        mappings.Should().HaveCount(20);

        server.Stop();
    }
}
#endif

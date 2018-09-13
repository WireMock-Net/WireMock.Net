using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NFluent;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerAdminTests
    {
        // For for AppVeyor + OpenCover
        private string GetCurrentFolder()
        {
            string current = Directory.GetCurrentDirectory();
            //if (!current.EndsWith("WireMock.Net.Tests"))
            //    return Path.Combine(current, "test", "WireMock.Net.Tests");

            return current;
        }

        [Fact]
        public void FluentMockServer_Admin_StartStop()
        {
            var server1 = FluentMockServer.Start("http://localhost:19091");

            Check.That(server1.Urls[0]).Equals("http://localhost:19091");

            server1.Stop();
        }

        [Fact]
        public void FluentMockServer_Admin_ResetMappings()
        {
            var server = FluentMockServer.Start();

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings");
            server.ReadStaticMappings(folder);

            Check.That(server.Mappings).HasSize(3);

            // Act
            server.ResetMappings();

            // Assert
            Check.That(server.Mappings).HasSize(0);
        }

        [Fact]
        public void FluentMockServer_Admin_SaveStaticMappings()
        {
            // Assign
            string guid = "791a3f31-6946-aaaa-8e6f-0237c7441111";
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.GetMappingFolder()).Returns("folder");
            staticMappingHandlerMock.Setup(m => m.FolderExists(It.IsAny<string>())).Returns(true);
            staticMappingHandlerMock.Setup(m => m.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()));

            var _server = FluentMockServer.Start(new FluentMockServerSettings
            {
                FileSystemHandler = staticMappingHandlerMock.Object
            });

            _server
                .Given(Request.Create().WithPath($"/foo_{Guid.NewGuid()}"))
                .WithGuid(guid)
                .RespondWith(Response.Create().WithBody("save test"));

            // Act
            _server.SaveStaticMappings();

            // Assert and Verify
            staticMappingHandlerMock.Verify(m => m.GetMappingFolder(), Times.Once);
            staticMappingHandlerMock.Verify(m => m.FolderExists("folder"), Times.Once);
            staticMappingHandlerMock.Verify(m => m.WriteMappingFile(Path.Combine("folder", guid + ".json"), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void FluentMockServer_Admin_ReadStaticMapping_WithNonGuidFilename()
        {
            var guid = Guid.Parse("04ee4872-9efd-4770-90d3-88d445265d0d");
            string title = "documentdb_root_title";

            var _server = FluentMockServer.Start();

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings", "documentdb_root.json");
            _server.ReadStaticMappingAndAddOrUpdate(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(guid);
            Check.That(mappings.First().Title).Equals(title);
        }

        [Fact]
        public void FluentMockServer_Admin_ReadStaticMapping_WithGuidFilename()
        {
            string guid = "00000002-ee28-4f29-ae63-1ac9b0802d86";

            var _server = FluentMockServer.Start();
            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings", guid + ".json");
            _server.ReadStaticMappingAndAddOrUpdate(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(Guid.Parse(guid));
            Check.That(mappings.First().Title).IsNullOrEmpty();
        }

        [Fact]
        public void FluentMockServer_Admin_ReadStaticMapping_WithResponseBodyFromFile()
        {
            string guid = "00000002-ee28-4f29-ae63-1ac9b0802d87";

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings", guid + ".json");
            string json = File.ReadAllText(folder);

            string responseBodyFilePath = Path.Combine(GetCurrentFolder(), "responsebody.json");

            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["Response"]["BodyAsFile"] = responseBodyFilePath;

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(folder, output);

            var _server = FluentMockServer.Start();
            _server.ReadStaticMappingAndAddOrUpdate(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(Guid.Parse(guid));
            Check.That(mappings.First().Title).IsNullOrEmpty();
        }

        [Fact]
        public void FluentMockServer_Admin_ReadStaticMappings_FolderExistsIsTrue()
        {
            // Assign
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.GetMappingFolder()).Returns("folder");
            staticMappingHandlerMock.Setup(m => m.FolderExists(It.IsAny<string>())).Returns(true);
            staticMappingHandlerMock.Setup(m => m.EnumerateFiles(It.IsAny<string>())).Returns(new string[0]);

            var _server = FluentMockServer.Start(new FluentMockServerSettings
            {
                FileSystemHandler = staticMappingHandlerMock.Object
            });

            // Act
            _server.ReadStaticMappings();

            // Assert and Verify
            staticMappingHandlerMock.Verify(m => m.GetMappingFolder(), Times.Once);
            staticMappingHandlerMock.Verify(m => m.FolderExists("folder"), Times.Once);
            staticMappingHandlerMock.Verify(m => m.EnumerateFiles("folder"), Times.Once);
        }

        [Fact]
        public void FluentMockServer_Admin_ReadStaticMappingAndAddOrUpdate()
        {
            // Assign
            string mapping = "{\"Request\": {\"Path\": {\"Matchers\": [{\"Name\": \"WildcardMatcher\",\"Pattern\": \"/static/mapping\"}]},\"Methods\": [\"get\"]},\"Response\": {\"BodyAsJson\": { \"body\": \"static mapping\" }}}";
            var _staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            _staticMappingHandlerMock.Setup(m => m.ReadMappingFile(It.IsAny<string>())).Returns(mapping);

            var _server = FluentMockServer.Start(new FluentMockServerSettings
            {
                FileSystemHandler = _staticMappingHandlerMock.Object
            });

            // Act
            _server.ReadStaticMappingAndAddOrUpdate(@"c:\test.json");

            // Assert and Verify
            _staticMappingHandlerMock.Verify(m => m.ReadMappingFile(@"c:\test.json"), Times.Once);
        }

        [Fact]
        public void FluentMockServer_Admin_ReadStaticMappings()
        {
            var _server = FluentMockServer.Start();

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings");
            _server.ReadStaticMappings(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(3);
        }

        [Fact]
        public void FluentMockServer_Admin_ReadStaticMappings_FolderDoesNotExist()
        {
            // Assign
            var loggerMock = new Mock<IWireMockLogger>();
            loggerMock.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()));
            var settings = new FluentMockServerSettings
            {
                Logger = loggerMock.Object
            };
            var _server = FluentMockServer.Start(settings);

            // Act
            _server.ReadStaticMappings(Guid.NewGuid().ToString());

            // Assert
            Check.That(_server.Mappings).HasSize(0);

            // Verify
            loggerMock.Verify(l => l.Info(It.Is<string>(s => s.StartsWith("The Static Mapping folder")), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public void FluentMockServer_Admin_Mappings_WithGuid_Get()
        {
            Guid guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath("/foo1").UsingGet()).WithGuid(guid)
                .RespondWith(Response.Create().WithStatusCode(201).WithBody("1"));

            _server.Given(Request.Create().WithPath("/foo2").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(202).WithBody("2"));

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);
        }

        [Fact]
        public void FluentMockServer_Admin_Mappings_WithGuidAsString_Get()
        {
            string guid = "90356dba-b36c-469a-a17e-669cd84f1f05";
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath("/foo100").UsingGet()).WithGuid(guid)
                .RespondWith(Response.Create().WithStatusCode(201).WithBody("1"));

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);
        }

        [Fact]
        public void FluentMockServer_Admin_Mappings_Add_SameGuid()
        {
            var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
            var _server = FluentMockServer.Start();

            var response1 = Response.Create().WithStatusCode(500);
            _server.Given(Request.Create().UsingGet())
                .WithGuid(guid)
                .RespondWith(response1);

            var mappings1 = _server.Mappings.ToArray();
            Check.That(mappings1).HasSize(1);
            Check.That(mappings1.First().Guid).Equals(guid);

            var response2 = Response.Create().WithStatusCode(400);
            _server.Given(Request.Create().WithPath("/2").UsingGet())
                .WithGuid(guid)
                .RespondWith(response2);

            var mappings2 = _server.Mappings.ToArray();
            Check.That(mappings2).HasSize(1);
            Check.That(mappings2.First().Guid).Equals(guid);
            Check.That(mappings2.First().Provider).Equals(response2);
        }

        [Fact]
        public async Task FluentMockServer_Admin_Mappings_AtPriority()
        {
            var _server = FluentMockServer.Start();

            // given
            _server.Given(Request.Create().WithPath("/1").UsingGet())
                .AtPriority(2)
                .RespondWith(Response.Create().WithStatusCode(200));

            _server.Given(Request.Create().WithPath("/1").UsingGet())
                .AtPriority(1)
                .RespondWith(Response.Create().WithStatusCode(400));

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/1");

            // then
            Check.That((int)response.StatusCode).IsEqualTo(400);
        }

        [Fact]
        public async Task FluentMockServer_Admin_Requests_Get()
        {
            // given
            var _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(_server.LogEntries).HasSize(1);
            var requestLogged = _server.LogEntries.First();
            Check.That(requestLogged.RequestMessage.Method).IsEqualTo("get");
            Check.That(requestLogged.RequestMessage.BodyAsBytes).IsNull();
        }

        [Fact]
        public async Task FluentMockServer_Admin_Logging_SetMaxRequestLogCount()
        {
            // Assign
            var client = new HttpClient();
            // Act
            var _server = FluentMockServer.Start();
            _server.SetMaxRequestLogCount(2);

            await client.GetAsync("http://localhost:" + _server.Ports[0] + "/foo1");
            await client.GetAsync("http://localhost:" + _server.Ports[0] + "/foo2");
            await client.GetAsync("http://localhost:" + _server.Ports[0] + "/foo3");

            // Assert
            Check.That(_server.LogEntries).HasSize(2);

            var requestLoggedA = _server.LogEntries.First();
            Check.That(requestLoggedA.RequestMessage.Path).EndsWith("/foo2");

            var requestLoggedB = _server.LogEntries.Last();
            Check.That(requestLoggedB.RequestMessage.Path).EndsWith("/foo3");
        }

        [Fact]
        public void FluentMockServer_Admin_WatchStaticMappings()
        {
            // Assign
            var fileMock = new Mock<IFileSystemHandler>();
            var settings = new FluentMockServerSettings
            {
                FileSystemHandler = fileMock.Object
            };
            var _server = FluentMockServer.Start(settings);

            // Act
            _server.WatchStaticMappings();

            // Verify
            fileMock.Verify(f => f.GetMappingFolder(), Times.Once);
            fileMock.Verify(f => f.FolderExists(It.IsAny<string>()), Times.Once);
        }
    }
}
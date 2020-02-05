﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
    public class WireMockServerAdminTests
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
        public void WireMockServer_Admin_StartStop()
        {
            var server1 = WireMockServer.Start("http://localhost:19091");

            Check.That(server1.Urls[0]).Equals("http://localhost:19091");

            server1.Stop();
        }

        [Fact]
        public void WireMockServer_Admin_ResetMappings()
        {
            var server = WireMockServer.Start();
            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings");
            server.ReadStaticMappings(folder);

            Check.That(server.Mappings).HasSize(5);
            Check.That(server.MappingModels).HasSize(5);

            // Act
            server.ResetMappings();

            // Assert
            Check.That(server.Mappings).HasSize(0);
            Check.That(server.MappingModels).HasSize(0);
        }

        [Fact]
        public void WireMockServer_Admin_SaveStaticMappings()
        {
            // Assign
            string guid = "791a3f31-6946-aaaa-8e6f-0237c7441111";
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.GetMappingFolder()).Returns("folder");
            staticMappingHandlerMock.Setup(m => m.FolderExists(It.IsAny<string>())).Returns(true);
            staticMappingHandlerMock.Setup(m => m.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                FileSystemHandler = staticMappingHandlerMock.Object
            });

            server
                .Given(Request.Create().WithPath($"/foo_{Guid.NewGuid()}"))
                .WithGuid(guid)
                .RespondWith(Response.Create().WithBody("save test"));

            // Act
            server.SaveStaticMappings();

            // Assert and Verify
            staticMappingHandlerMock.Verify(m => m.GetMappingFolder(), Times.Once);
            staticMappingHandlerMock.Verify(m => m.FolderExists("folder"), Times.Once);
            staticMappingHandlerMock.Verify(m => m.WriteMappingFile(Path.Combine("folder", guid + ".json"), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMapping_WithNonGuidFilename()
        {
            var guid = Guid.Parse("04ee4872-9efd-4770-90d3-88d445265d0d");
            string title = "documentdb_root_title";

            var server = WireMockServer.Start();

            string path = Path.Combine(GetCurrentFolder(), "__admin", "mappings", "documentdb_root.json");
            server.ReadStaticMappingAndAddOrUpdate(path);

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(guid);
            Check.That(mappings.First().Title).Equals(title);
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMapping_WithGuidFilename()
        {
            string guid = "00000002-ee28-4f29-ae63-1ac9b0802d86";

            var server = WireMockServer.Start();
            string path = Path.Combine(GetCurrentFolder(), "__admin", "mappings", guid + ".json");
            server.ReadStaticMappingAndAddOrUpdate(path);

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(Guid.Parse(guid));
            Check.That(mappings.First().Title).IsNullOrEmpty();
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMapping_WithArray()
        {
            var server = WireMockServer.Start();

            string path = Path.Combine(GetCurrentFolder(), "__admin", "mappings", "array.json");
            server.ReadStaticMappingAndAddOrUpdate(path);

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMapping_WithResponseBodyFromFile()
        {
            string guid = "00000002-ee28-4f29-ae63-1ac9b0802d87";

            string path = Path.Combine(GetCurrentFolder(), "__admin", "mappings", guid + ".json");
            string json = File.ReadAllText(path);

            string responseBodyFilePath = Path.Combine(GetCurrentFolder(), "responsebody.json");

            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["Response"]["BodyAsFile"] = responseBodyFilePath;

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(path, output);

            var server = WireMockServer.Start();
            server.ReadStaticMappingAndAddOrUpdate(path);

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);

            Check.That(mappings.First().RequestMatcher).IsNotNull();
            Check.That(mappings.First().Provider).IsNotNull();
            Check.That(mappings.First().Guid).Equals(Guid.Parse(guid));
            Check.That(mappings.First().Title).IsNullOrEmpty();
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMappings_FolderExistsIsTrue()
        {
            // Assign
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.GetMappingFolder()).Returns("folder");
            staticMappingHandlerMock.Setup(m => m.FolderExists(It.IsAny<string>())).Returns(true);
            staticMappingHandlerMock.Setup(m => m.EnumerateFiles(It.IsAny<string>(), It.IsAny<bool>())).Returns(new string[0]);

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                FileSystemHandler = staticMappingHandlerMock.Object
            });

            // Act
            server.ReadStaticMappings();

            // Assert and Verify
            staticMappingHandlerMock.Verify(m => m.GetMappingFolder(), Times.Once);
            staticMappingHandlerMock.Verify(m => m.FolderExists("folder"), Times.Once);
            staticMappingHandlerMock.Verify(m => m.EnumerateFiles("folder", false), Times.Once);
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMappingAndAddOrUpdate()
        {
            // Assign
            string mapping = "{\"Request\": {\"Path\": {\"Matchers\": [{\"Name\": \"WildcardMatcher\",\"Pattern\": \"/static/mapping\"}]},\"Methods\": [\"get\"]},\"Response\": {\"BodyAsJson\": { \"body\": \"static mapping\" }}}";
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.ReadMappingFile(It.IsAny<string>())).Returns(mapping);

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                FileSystemHandler = staticMappingHandlerMock.Object
            });

            // Act
            server.ReadStaticMappingAndAddOrUpdate(@"c:\test.json");

            // Assert and Verify
            staticMappingHandlerMock.Verify(m => m.ReadMappingFile(@"c:\test.json"), Times.Once);
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMappings()
        {
            var server = WireMockServer.Start();

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings");
            server.ReadStaticMappings(folder);

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(5);
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticMappings_FolderDoesNotExist()
        {
            // Assign
            var loggerMock = new Mock<IWireMockLogger>();
            loggerMock.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<object[]>()));
            var settings = new WireMockServerSettings
            {
                Logger = loggerMock.Object
            };
            var server = WireMockServer.Start(settings);

            // Act
            server.ReadStaticMappings(Guid.NewGuid().ToString());

            // Assert
            Check.That(server.Mappings).HasSize(0);

            // Verify
            loggerMock.Verify(l => l.Info(It.Is<string>(s => s.StartsWith("The Static Mapping folder")), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public void WireMockServer_Admin_Mappings_WithGuid_Get()
        {
            Guid guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
            var server = WireMockServer.Start();

            server.Given(Request.Create().WithPath("/foo1").UsingGet()).WithGuid(guid)
                .RespondWith(Response.Create().WithStatusCode(201).WithBody("1"));

            server.Given(Request.Create().WithPath("/foo2").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(202).WithBody("2"));

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);
        }

        [Fact]
        public void WireMockServer_Admin_Mappings_WithGuidAsString_Get()
        {
            string guid = "90356dba-b36c-469a-a17e-669cd84f1f05";
            var server = WireMockServer.Start();

            server.Given(Request.Create().WithPath("/foo100").UsingGet()).WithGuid(guid)
                .RespondWith(Response.Create().WithStatusCode(201).WithBody("1"));

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(1);
        }

        [Fact]
        public void WireMockServer_Admin_Mappings_Add_SameGuid()
        {
            var guid = Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05");
            var server = WireMockServer.Start();

            var response1 = Response.Create().WithStatusCode(500);
            server.Given(Request.Create().UsingGet())
                .WithGuid(guid)
                .RespondWith(response1);

            var mappings1 = server.Mappings.ToArray();
            Check.That(mappings1).HasSize(1);
            Check.That(mappings1.First().Guid).Equals(guid);

            var response2 = Response.Create().WithStatusCode(400);
            server.Given(Request.Create().WithPath("/2").UsingGet())
                .WithGuid(guid)
                .RespondWith(response2);

            var mappings2 = server.Mappings.ToArray();
            Check.That(mappings2).HasSize(1);
            Check.That(mappings2.First().Guid).Equals(guid);
            Check.That(mappings2.First().Provider).Equals(response2);
        }

        [Fact]
        public async Task WireMockServer_Admin_Mappings_AtPriority()
        {
            var server = WireMockServer.Start();

            // given
            server.Given(Request.Create().WithPath("/1").UsingGet())
                .AtPriority(2)
                .RespondWith(Response.Create().WithStatusCode(200));

            server.Given(Request.Create().WithPath("/1").UsingGet())
                .AtPriority(1)
                .RespondWith(Response.Create().WithStatusCode(400));

            var mappings = server.Mappings.ToArray();
            Check.That(mappings).HasSize(2);

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + server.Ports[0] + "/1");

            // then
            Check.That((int)response.StatusCode).IsEqualTo(400);
        }

        [Fact]
        public async Task WireMockServer_Admin_Requests_Get()
        {
            // given
            var server = WireMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + server.Ports[0] + "/foo");

            // then
            Check.That(server.LogEntries).HasSize(1);
            var requestLogged = server.LogEntries.First();
            Check.That(requestLogged.RequestMessage.Method).IsEqualTo("GET");
            Check.That(requestLogged.RequestMessage.BodyData).IsNull();
        }

        [Fact]
        public async Task WireMockServer_Admin_Logging_SetMaxRequestLogCount()
        {
            // Assign
            var client = new HttpClient();
            // Act
            var server = WireMockServer.Start();
            server.SetMaxRequestLogCount(2);

            await client.GetAsync("http://localhost:" + server.Ports[0] + "/foo1");
            await client.GetAsync("http://localhost:" + server.Ports[0] + "/foo2");
            await client.GetAsync("http://localhost:" + server.Ports[0] + "/foo3");

            // Assert
            Check.That(server.LogEntries).HasSize(2);

            var requestLoggedA = server.LogEntries.First();
            Check.That(requestLoggedA.RequestMessage.Path).EndsWith("/foo2");

            var requestLoggedB = server.LogEntries.Last();
            Check.That(requestLoggedB.RequestMessage.Path).EndsWith("/foo3");
        }

        [Fact]
        public void WireMockServer_Admin_WatchStaticMappings()
        {
            // Assign
            var fileMock = new Mock<IFileSystemHandler>();
            var settings = new WireMockServerSettings
            {
                FileSystemHandler = fileMock.Object
            };
            var server = WireMockServer.Start(settings);

            // Act
            server.WatchStaticMappings();

            // Verify
            fileMock.Verify(f => f.GetMappingFolder(), Times.Once);
            fileMock.Verify(f => f.FolderExists(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void WireMockServer_Admin_AddMappingsAndSaveToFile()
        {
            // Assign
            string guid = "791a3f31-6946-aaaa-8e6f-0237c7442222";
            var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
            staticMappingHandlerMock.Setup(m => m.GetMappingFolder()).Returns("folder");
            staticMappingHandlerMock.Setup(m => m.FolderExists(It.IsAny<string>())).Returns(true);
            staticMappingHandlerMock.Setup(m => m.WriteMappingFile(It.IsAny<string>(), It.IsAny<string>()));

            var server = WireMockServer.Start(new WireMockServerSettings
            {
                FileSystemHandler = staticMappingHandlerMock.Object
            });

            // Act
            server
                .Given(Request.Create().WithPath($"/foo_{Guid.NewGuid()}"), true)
                .WithGuid(guid)
                .RespondWith(Response.Create().WithBody("post and save test"));

            // Assert and Verify
            staticMappingHandlerMock.Verify(m => m.GetMappingFolder(), Times.Once);
            staticMappingHandlerMock.Verify(m => m.FolderExists("folder"), Times.Once);
            staticMappingHandlerMock.Verify(m => m.WriteMappingFile(Path.Combine("folder", guid + ".json"), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void WireMockServer_Admin_DeleteMappings()
        {
            // Arrange
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = true,
                ReadStaticMappings = false,
                WatchStaticMappings = false,
                WatchStaticMappingsInSubdirectories = false
            });

            server
                .Given(Request.Create().WithPath("/path1"))
                .AtPriority(0)
                .RespondWith(Response.Create().WithStatusCode(200));
            server
                .Given(Request.Create().WithPath("/path2"))
                .AtPriority(1)
                .RespondWith(Response.Create().WithStatusCode(200));
            server
                .Given(Request.Create().WithPath("/path3"))
                .AtPriority(2)
                .RespondWith(Response.Create().WithStatusCode(200));

            Check.That(server.MappingModels.Count()).Equals(3);

            Guid? guid1 = server.MappingModels.ElementAt(0).Guid;
            Guid? guid2 = server.MappingModels.ElementAt(1).Guid;
            Guid? guid3 = server.MappingModels.ElementAt(2).Guid;

            Check.That(guid1).IsNotNull();
            Check.That(guid2).IsNotNull();
            Check.That(guid3).IsNotNull();

            string guidsJsonBody = $"[" +
                                     $"{{\"Guid\": \"{guid1}\"}}," +
                                     $"{{\"Guid\": \"{guid2}\"}}" +
                                   $"]";

            // Act
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"http://localhost:{server.Ports[0]}/__admin/mappings"),
                Content = new StringContent(guidsJsonBody, Encoding.UTF8, "application/json")
            };

            var response = await new HttpClient().SendAsync(request);

            // Assert
            IEnumerable<Guid> guids = server.MappingModels.Select(mapping => mapping.Guid.Value);
            Check.That(guids.Contains(guid1.Value)).IsFalse();
            Check.That(guids.Contains(guid2.Value)).IsFalse();
            Check.That(guids.Contains(guid3.Value)).IsTrue();
            Check.That(response.StatusCode).Equals(HttpStatusCode.OK);
            Check.That(await response.Content.ReadAsStringAsync()).Equals($"{{\"Status\":\"Mappings deleted. Affected GUIDs: [{guid1}, {guid2}]\"}}");
        }
    }
}
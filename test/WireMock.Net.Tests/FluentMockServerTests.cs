using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NFluent;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using Newtonsoft.Json;
using WireMock.Handlers;
using WireMock.Settings;

namespace WireMock.Net.Tests
{
    public partial class FluentMockServerTests
    {
        private static string jsonRequestMessage = @"{ ""message"" : ""Hello server"" }";

        // For for AppVeyor + OpenCover
        private string GetCurrentFolder()
        {
            string current = Directory.GetCurrentDirectory();
            //if (!current.EndsWith("WireMock.Net.Tests"))
            //    return Path.Combine(current, "test", "WireMock.Net.Tests");

            return current;
        }

        [Fact]
        public void FluentMockServer_StartStop()
        {
            var server1 = FluentMockServer.Start("http://localhost:9091/");
            server1.Stop();

            var server2 = FluentMockServer.Start("http://localhost:9091/");
            server2.Stop();
        }

        [Fact]
        public void FluentMockServer_SaveStaticMappings()
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
        public void FluentMockServer_ReadStaticMapping_WithNonGuidFilename()
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
        public void FluentMockServer_ReadStaticMapping_WithGuidFilename()
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
        public void FluentMockServer_ReadStaticMapping_WithResponseBodyFromFile()
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
        public void FluentMockServer_ReadStaticMappings_FolderExistsIsTrue()
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
        public void FluentMockServer_ReadStaticMappingAndAddOrUpdate()
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
        public void FluentMockServer_ReadStaticMappings()
        {
            var _server = FluentMockServer.Start();

            string folder = Path.Combine(GetCurrentFolder(), "__admin", "mappings");
            _server.ReadStaticMappings(folder);

            var mappings = _server.Mappings.ToArray();
            Check.That(mappings).HasSize(3);
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
        public async Task FluentMockServer_Should_respond_to_request_methodPatch()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath(path).UsingMethod("patch"))
                .RespondWith(Response.Create().WithBody("hello patch"));

            // when
            var msg = new HttpRequestMessage(new HttpMethod("patch"), new Uri("http://localhost:" + _server.Ports[0] + path))
            {
                Content = new StringContent("{\"data\": {\"attr\":\"value\"}}")
            };
            var response = await new HttpClient().SendAsync(msg);

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            Check.That(responseBody).IsEqualTo("hello patch");

            Check.That(_server.LogEntries).HasSize(1);
            var requestLogged = _server.LogEntries.First();
            Check.That(requestLogged.RequestMessage.Method).IsEqualTo("patch");
            Check.That(requestLogged.RequestMessage.Body).IsNotNull();
            Check.That(requestLogged.RequestMessage.Body).IsEqualTo("{\"data\": {\"attr\":\"value\"}}");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsString()
        {
            // given
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("Hello world!"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response).IsEqualTo("Hello world!");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_BodyAsJson()
        {
            // Assign
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().UsingAnyMethod())
                .RespondWith(Response.Create().WithBodyAsJson(new { message = "Hello" }));

            // Act
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0]);

            // Assert
            Check.That(response).IsEqualTo("{\"message\":\"Hello\"}");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_BodyAsJson_Indented()
        {
            // Assign
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().UsingAnyMethod())
                .RespondWith(Response.Create().WithBodyAsJson(new { message = "Hello" }, true));

            // Act
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0]);

            // Assert
            Check.That(response).IsEqualTo("{\r\n  \"message\": \"Hello\"\r\n}");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsCallback()
        {
            // given
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/foo")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(req => $"{{ path: '{req.Path}' }}"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response).IsEqualTo("{ path: '/foo' }");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsBase64()
        {
            // given
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath("/foo").UsingGet()).RespondWith(Response.Create().WithBodyFromBase64("SGVsbG8gV29ybGQ/"));

            // when
            var response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // then
            Check.That(response).IsEqualTo("Hello World?");
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_request_bodyAsBytes()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server.Given(Request.Create().WithPath(path).UsingGet()).RespondWith(Response.Create().WithBody(new byte[] { 48, 49 }));

            // when
            var responseAsString = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + path);
            var responseAsBytes = await new HttpClient().GetByteArrayAsync("http://localhost:" + _server.Ports[0] + path);

            // then
            Check.That(responseAsString).IsEqualTo("01");
            Check.That(responseAsBytes).ContainsExactly(new byte[] { 48, 49 });
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_to_valid_matchers_when_sent_json()
        {
            // Assign
            var validMatchersForHelloServerJsonMessage = new List<object[]>
            {
                new object[] { new WildcardMatcher("*Hello server*"), "application/json" },
                new object[] { new WildcardMatcher("*Hello server*"), "text/plain" },
                new object[] { new ExactMatcher(jsonRequestMessage), "application/json" },
                new object[] { new ExactMatcher(jsonRequestMessage), "text/plain" },
                new object[] { new RegexMatcher("Hello server"), "application/json" },
                new object[] { new RegexMatcher("Hello server"), "text/plain" },
                new object[] { new JsonPathMatcher("$..[?(@.message == 'Hello server')]"), "application/json" },
                new object[] { new JsonPathMatcher("$..[?(@.message == 'Hello server')]"), "text/plain" }
            };

            var _server = FluentMockServer.Start();

            foreach (var item in validMatchersForHelloServerJsonMessage)
            {
                string path = $"/foo_{Guid.NewGuid()}";
                _server
                    .Given(Request.Create().WithPath(path).WithBody((IMatcher)item[0]))
                    .RespondWith(Response.Create().WithBody("Hello client"));

                // Act
                var content = new StringContent(jsonRequestMessage, Encoding.UTF8, (string)item[1]);
                var response = await new HttpClient().PostAsync("http://localhost:" + _server.Ports[0] + path, content);

                // Assert
                var responseString = await response.Content.ReadAsStringAsync();
                Check.That(responseString).Equals("Hello client");

                _server.ResetMappings();
                _server.ResetLogEntries();
            }
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_404_for_unexpected_request()
        {
            // given
            string path = $"/foo{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            // when
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + path);

            // then
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
            Check.That((int)response.StatusCode).IsEqualTo(404);
        }

        [Fact]
        public async Task FluentMockServer_Should_find_a_request_satisfying_a_request_spec()
        {
            // Assign
            string path = $"/bar_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + path);

            // then
            var result = _server.FindLogEntries(Request.Create().WithPath(new RegexMatcher("^/b.*"))).ToList();
            Check.That(result).HasSize(1);

            var requestLogged = result.First();
            Check.That(requestLogged.RequestMessage.Path).IsEqualTo(path);
            Check.That(requestLogged.RequestMessage.Url).IsEqualTo("http://localhost:" + _server.Ports[0] + path);
        }

        [Fact]
        public async Task FluentMockServer_Should_reset_requestlogs()
        {
            // given
            var _server = FluentMockServer.Start();

            // when
            await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + "/foo");
            _server.ResetLogEntries();

            // then
            Check.That(_server.LogEntries).IsEmpty();
        }

        [Fact]
        public void FluentMockServer_Should_reset_mappings()
        {
            // given
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}"));

            // when
            _server.ResetMappings();

            // then
            Check.That(_server.Mappings).IsEmpty();
            Check.ThatAsyncCode(() => new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + path))
                .ThrowsAny();
        }

        [Fact]
        public async Task FluentMockServer_Should_respond_a_redirect_without_body()
        {
            // Assign
            string path = $"/foo_{Guid.NewGuid()}";
            string pathToRedirect = $"/bar_{Guid.NewGuid()}";

            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath(path)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(307)
                    .WithHeader("Location", pathToRedirect));
            _server
                .Given(Request.Create()
                    .WithPath(pathToRedirect)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("REDIRECT SUCCESSFUL"));

            // Act
            var response = await new HttpClient().GetStringAsync($"http://localhost:{_server.Ports[0]}{path}");

            // Assert
            Check.That(response).IsEqualTo("REDIRECT SUCCESSFUL");
        }

        [Fact]
        public async Task FluentMockServer_Should_delay_responses_for_a_given_route()
        {
            // given
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create()
                    .WithPath("/*"))
                .RespondWith(Response.Create()
                    .WithBody(@"{ msg: ""Hello world!""}")
                    .WithDelay(TimeSpan.FromMilliseconds(200)));

            // when
            var watch = new Stopwatch();
            watch.Start();
            await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            watch.Stop();

            // then
            Check.That(watch.ElapsedMilliseconds).IsStrictlyGreaterThan(200);
        }

        [Fact]
        public async Task FluentMockServer_Should_delay_responses()
        {
            // given
            var _server = FluentMockServer.Start();
            _server.AddGlobalProcessingDelay(TimeSpan.FromMilliseconds(200));
            _server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithBody(@"{ msg: ""Hello world!""}"));

            // when
            var watch = new Stopwatch();
            watch.Start();
            await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");
            watch.Stop();

            // then
            Check.That(watch.ElapsedMilliseconds).IsStrictlyGreaterThan(200);
        }

        //Leaving commented as this requires an actual certificate with password, along with a service that expects a client certificate
        //[Fact]
        //public async Task Should_proxy_responses_with_client_certificate()
        //{
        //    // given
        //    var _server = FluentMockServer.Start();
        //    _server
        //        .Given(Request.Create().WithPath("/*"))
        //        .RespondWith(Response.Create().WithProxy("https://server-that-expects-a-client-certificate", @"\\yourclientcertificatecontainingprivatekey.pfx", "yourclientcertificatepassword"));

        //    // when
        //    var result = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/someurl?someQuery=someValue");

        //    // then
        //    Check.That(result).Contains("google");
        //}

        [Fact]
        public async Task FluentMockServer_Logging_SetMaxRequestLogCount()
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
        public async Task FluentMockServer_Should_respond_to_request_callback()
        {
            // Assign
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().WithPath("/foo").UsingGet())
                .RespondWith(Response.Create().WithCallback(req => new ResponseMessage { Body = req.Path + "Bar" }));

            // Act
            string response = await new HttpClient().GetStringAsync("http://localhost:" + _server.Ports[0] + "/foo");

            // Assert
            Check.That(response).IsEqualTo("/fooBar");
        }

#if !NET452
        [Fact]
        public async Task FluentMockServer_Should_not_exclude_restrictedResponseHeader_for_ASPNETCORE()
        {
            // Assign
            string path = $"/foo_{Guid.NewGuid()}";
            var _server = FluentMockServer.Start();

            _server
                .Given(Request.Create().WithPath(path).UsingGet())
                .RespondWith(Response.Create().WithHeader("Keep-Alive", "k").WithHeader("test", "t"));

            // Act
            var response = await new HttpClient().GetAsync("http://localhost:" + _server.Ports[0] + path);

            // Assert
            Check.That(response.Headers.Contains("test")).IsTrue();
            Check.That(response.Headers.Contains("Keep-Alive")).IsTrue();
        }
#endif
    }
}
using NFluent;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerProxyTests
    {
        [Fact]
        public async Task FluentMockServer_Proxy_Should_proxy_responses()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var server = FluentMockServer.Start();
            server
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create().WithProxy("http://www.google.com"));

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{server.Urls[0]}{path}")
            };
            var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
            var response = await new HttpClient(httpClientHandler).SendAsync(requestMessage);
            string content = await response.Content.ReadAsStringAsync();

            // Assert
            Check.That(server.Mappings).HasSize(1);
            Check.That(content).Contains("google");
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_preserve_content_header_in_proxied_request()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create());

            var settings = new FluentMockServerSettings
            {
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = serverForProxyForwarding.Urls[0],
                    SaveMapping = true,
                    SaveMappingToFile = false
                }
            };
            var server = FluentMockServer.Start(settings);

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{server.Urls[0]}{path}"),
                Content = new StringContent("stringContent", Encoding.ASCII)
            };
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            requestMessage.Content.Headers.Add("bbb", "test");
            await new HttpClient().SendAsync(requestMessage);

            // Assert
            var receivedRequest = serverForProxyForwarding.LogEntries.First().RequestMessage;
            Check.That(receivedRequest.BodyData.BodyAsString).IsEqualTo("stringContent");
            Check.That(receivedRequest.Headers).ContainsKey("Content-Type");
            Check.That(receivedRequest.Headers["Content-Type"].First()).Contains("text/plain");
            Check.That(receivedRequest.Headers).ContainsKey("bbb");

            // check that new proxied mapping is added
            Check.That(server.Mappings).HasSize(2);

            //var newMapping = _server.Mappings.First(m => m.Guid != guid);
            //var matcher = ((Request)newMapping.RequestMatcher).GetRequestMessageMatchers<RequestMessageHeaderMatcher>().FirstOrDefault(m => m.Name == "bbb");
            //Check.That(matcher).IsNotNull();
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_exclude_blacklisted_content_header_in_mapping()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create());

            var settings = new FluentMockServerSettings
            {
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = serverForProxyForwarding.Urls[0],
                    SaveMapping = true,
                    SaveMappingToFile = false,
                    BlackListedHeaders = new[] { "blacklisted" }
                }
            };
            var server = FluentMockServer.Start(settings);
            var defaultMapping = server.Mappings.First();

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{server.Urls[0]}{path}"),
                Content = new StringContent("stringContent")
            };
            requestMessage.Headers.Add("blacklisted", "exact_match");
            requestMessage.Headers.Add("ok", "ok-value");
            await new HttpClient().SendAsync(requestMessage);

            // Assert
            var mapping = server.Mappings.FirstOrDefault(m => m.Guid != defaultMapping.Guid);
            Check.That(mapping).IsNotNull();
            var matchers = ((Request)mapping.RequestMatcher).GetRequestMessageMatchers<RequestMessageHeaderMatcher>().Select(m => m.Name).ToList();
            Check.That(matchers).Not.Contains("blacklisted");
            Check.That(matchers).Contains("ok");
        }        
        
        [Fact]
        public async Task FluentMockServer_Proxy_Should_exclude_blacklisted_cookies_in_mapping()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create());

            var settings = new FluentMockServerSettings
            {
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = serverForProxyForwarding.Urls[0],
                    SaveMapping = true,
                    SaveMappingToFile = false,
                    BlackListedCookies = new[] { "ASP.NET_SessionId" }
                }
            };
            var server = FluentMockServer.Start(settings);
            var defaultMapping = server.Mappings.First();

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{server.Urls[0]}{path}"),
                Content = new StringContent("stringContent")
            };

            var cookieContainer = new CookieContainer(3);
            cookieContainer.Add(new Uri("http://localhost"), new Cookie("ASP.NET_SessionId", "exact_match"));
            cookieContainer.Add(new Uri("http://localhost"), new Cookie("AsP.NeT_SessIonID", "case_mismatch"));
            cookieContainer.Add(new Uri("http://localhost"), new Cookie("GoodCookie", "I_should_pass"));
            
            var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            await new HttpClient(handler).SendAsync(requestMessage);

            // Assert
            var mapping = server.Mappings.FirstOrDefault(m => m.Guid != defaultMapping.Guid);
            Check.That(mapping).IsNotNull();

            var matchers = ((Request)mapping.RequestMatcher).GetRequestMessageMatchers<RequestMessageCookieMatcher>().Select(m => m.Name).ToList();
            Check.That(matchers).Not.Contains("ASP.NET_SessionId");
            Check.That(matchers).Not.Contains("AsP.NeT_SessIonID");
            Check.That(matchers).Contains("GoodCookie");
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_preserve_content_header_in_proxied_request_with_empty_content()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create());

            var server = FluentMockServer.Start();
            server
                .Given(Request.Create().WithPath("/*"))
                .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{server.Urls[0]}{path}"),
                Content = new StringContent("")
            };
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            await new HttpClient().SendAsync(requestMessage);

            // Assert
            var receivedRequest = serverForProxyForwarding.LogEntries.First().RequestMessage;
            Check.That(receivedRequest.BodyData.BodyAsString).IsEqualTo("");
            Check.That(receivedRequest.Headers).ContainsKey("Content-Type");
            Check.That(receivedRequest.Headers["Content-Type"].First()).Contains("text/plain");
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_preserve_content_header_in_proxied_response()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create()
                    .WithBody("body")
                    .WithHeader("Content-Type", "text/plain"));

            var server = FluentMockServer.Start();
            server
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{server.Urls[0]}{path}")
            };
            var response = await new HttpClient().SendAsync(requestMessage);

            // Assert
            Check.That(await response.Content.ReadAsStringAsync()).IsEqualTo("body");
            Check.That(response.Content.Headers.Contains("Content-Type")).IsTrue();
            Check.That(response.Content.Headers.GetValues("Content-Type")).ContainsExactly("text/plain");
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_change_absolute_location_header_in_proxied_response()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var settings = new FluentMockServerSettings { AllowPartialMapping = false };

            var serverForProxyForwarding = FluentMockServer.Start(settings);
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.Redirect)
                    .WithHeader("Location", "/testpath"));

            var server = FluentMockServer.Start(settings);
            server
                .Given(Request.Create().WithPath(path).UsingAnyMethod())
                .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{server.Urls[0]}{path}")
            };
            var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false };
            var response = await new HttpClient(httpClientHandler).SendAsync(requestMessage);

            // Assert
            Check.That(response.Headers.Contains("Location")).IsTrue();
            Check.That(response.Headers.GetValues("Location")).ContainsExactly("/testpath");
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_preserve_cookie_header_in_proxied_request()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create());

            var server = FluentMockServer.Start();
            server
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

            // Act
            var requestUri = new Uri($"{server.Urls[0]}{path}");
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = requestUri
            };
            var clientHandler = new HttpClientHandler();
            clientHandler.CookieContainer.Add(requestUri, new Cookie("name", "value"));
            await new HttpClient(clientHandler).SendAsync(requestMessage);

            // then
            var receivedRequest = serverForProxyForwarding.LogEntries.First().RequestMessage;
            Check.That(receivedRequest.Cookies).IsNotNull();
            Check.That(receivedRequest.Cookies).ContainsPair("name", "value");
        }

        /// <summary>
        /// Send some binary content in a request through the proxy and check that the same content
        /// arrived at the target. As example a JPEG/JIFF header is used, which is not representable
        /// in UTF8 and breaks if it is not treated as binary content. 
        /// </summary>
        [Fact]
        public async Task FluentMockServer_Proxy_Should_preserve_binary_request_content()
        {
            // arrange
            var jpegHeader = new byte[] {0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00};
            var brokenJpegHeader = new byte[]
                {0xEF, 0xBF, 0xBD, 0xEF, 0xBF, 0xBD, 0xEF, 0xBF, 0xBD, 0xEF, 0xBF, 0xBD, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00};

            bool HasCorrectHeader(byte[] bytes) => bytes.SequenceEqual(jpegHeader);
            bool HasBrokenHeader(byte[] bytes) => bytes.SequenceEqual(brokenJpegHeader);
            
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithBody(HasCorrectHeader))
                .RespondWith(Response.Create().WithSuccess());

            serverForProxyForwarding
                .Given(Request.Create().WithBody(HasBrokenHeader))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.InternalServerError));
            
            var server = FluentMockServer.Start();
            server
                .Given(Request.Create())
                .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

            // act
            var response = await new HttpClient().PostAsync(server.Urls[0], new ByteArrayContent(jpegHeader));
            
            // assert
            Check.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_set_BodyAsJson_in_proxied_response()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create()
                    .WithBodyAsJson(new { i = 42 })
                    .WithHeader("Content-Type", "application/json; charset=utf-8"));

            var server = FluentMockServer.Start();
            server
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

            // Act
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{server.Urls[0]}{path}")
            };
            var response = await new HttpClient().SendAsync(requestMessage);

            // Assert
            string content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo("{\"i\":42}");
            Check.That(response.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/json; charset=utf-8");
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_set_Body_in_multipart_proxied_response()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create()
                    .WithBodyAsJson(new { i = 42 })
            );

            var server = FluentMockServer.Start();
            server
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create().WithProxy(serverForProxyForwarding.Urls[0]));

            // Act
            var uri = new Uri($"{server.Urls[0]}{path}");
            var form = new MultipartFormDataContent
            {
                { new StringContent("data"), "test", "test.txt" }
            };
            var response = await new HttpClient().PostAsync(uri, form);

            // Assert
            string content = await response.Content.ReadAsStringAsync();
            Check.That(content).IsEqualTo("{\"i\":42}");
        }

        [Fact]
        public async Task FluentMockServer_Proxy_Should_Not_overrule_AdminMappings()
        {
            // Assign
            string path = $"/prx_{Guid.NewGuid().ToString()}";
            var serverForProxyForwarding = FluentMockServer.Start();
            serverForProxyForwarding
                .Given(Request.Create().WithPath(path))
                .RespondWith(Response.Create().WithBody("ok"));

            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                StartAdminInterface = true,
                ReadStaticMappings = false,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = serverForProxyForwarding.Urls[0],
                    SaveMapping = false,
                    SaveMappingToFile = false
                }
            });

            // Act 1
            var requestMessage1 = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{server.Urls[0]}{path}")
            };
            var response1 = await new HttpClient().SendAsync(requestMessage1);

            // Assert 1
            string content1 = await response1.Content.ReadAsStringAsync();
            Check.That(content1).IsEqualTo("ok");

            // Act 2
            var requestMessage2 = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{server.Urls[0]}/__admin/mappings")
            };
            var response2 = await new HttpClient().SendAsync(requestMessage2);

            // Assert 2
            string content2 = await response2.Content.ReadAsStringAsync();
            Check.That(content2).IsEqualTo("[]");
        }
    }
}
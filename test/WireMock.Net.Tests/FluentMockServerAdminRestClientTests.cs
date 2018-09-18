using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NFluent;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Admin.Settings;
using WireMock.Client;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests
{
    public class FluentMockServerAdminRestClientTests
    {
        [Fact]
        public async Task IFluentMockServerAdmin_GetSettingsAsync()
        {
            // Assign
            var server = FluentMockServer.StartWithAdminInterface();
            var api = RestClient.For<IFluentMockServerAdmin>(server.Urls[0]);

            // Act
            var settings = await api.GetSettingsAsync();
            Check.That(settings).IsNotNull();
        }

        [Fact]
        public async Task IFluentMockServerAdmin_PostSettingsAsync()
        {
            // Assign
            var server = FluentMockServer.StartWithAdminInterface();
            var api = RestClient.For<IFluentMockServerAdmin>(server.Urls[0]);

            // Act
            var settings = new SettingsModel();
            var status = await api.PostSettingsAsync(settings);
            Check.That(status.Status).Equals("Settings updated");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_PutSettingsAsync()
        {
            // Assign
            var server = FluentMockServer.StartWithAdminInterface();
            var api = RestClient.For<IFluentMockServerAdmin>(server.Urls[0]);

            // Act
            var settings = new SettingsModel();
            var status = await api.PutSettingsAsync(settings);
            Check.That(status.Status).Equals("Settings updated");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_PostMappingAsync()
        {
            // Assign
            var server = FluentMockServer.StartWithAdminInterface();
            var api = RestClient.For<IFluentMockServerAdmin>(server.Urls[0]);

            // Act
            var model = new MappingModel
            {
                Request = new RequestModel
                {
                    Path = "/1"
                },
                Response = new ResponseModel
                {
                    Body = "txt",
                    StatusCode = 200
                },
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
        public async Task IFluentMockServerAdmin_FindRequestsAsync()
        {
            // given
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                StartAdminInterface = true,
                Logger = new WireMockNullLogger()
            });
            var serverUrl = "http://localhost:" + server.Ports[0];
            await new HttpClient().GetAsync(serverUrl + "/foo");
            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.FindRequestsAsync(new RequestModel { Methods = new[] { "GET" } });

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("GET");
            Check.That(requestLogged.Request.Body).IsNull();
            Check.That(requestLogged.Request.Path).IsEqualTo("/foo");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_GetRequestsAsync()
        {
            // given
            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                StartAdminInterface = true,
                Logger = new WireMockNullLogger()
            });
            var serverUrl = "http://localhost:" + server.Ports[0];
            await new HttpClient().GetAsync(serverUrl + "/foo");
            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.GetRequestsAsync();

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("GET");
            Check.That(requestLogged.Request.Body).IsNull();
            Check.That(requestLogged.Request.Path).IsEqualTo("/foo");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_GetRequestsAsync_JsonApi()
        {
            // given
            var server = FluentMockServer.Start(new FluentMockServerSettings
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

            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.GetRequestsAsync();

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("POST");
            Check.That(requestLogged.Request.Body).IsNotNull();
            Check.That(requestLogged.Request.Body).Contains("T000001");
        }

        [Fact]
        public async Task IFluentMockServerAdmin_GetRequestsAsync_Json()
        {
            // given
            var server = FluentMockServer.Start(new FluentMockServerSettings
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

            var api = RestClient.For<IFluentMockServerAdmin>(serverUrl);

            // when
            var requests = await api.GetRequestsAsync();

            // then
            Check.That(requests).HasSize(1);
            var requestLogged = requests.First();
            Check.That(requestLogged.Request.Method).IsEqualTo("POST");
            Check.That(requestLogged.Request.Body).IsNotNull();
            Check.That(requestLogged.Request.Body).Contains("T000001");
        }
    }

}
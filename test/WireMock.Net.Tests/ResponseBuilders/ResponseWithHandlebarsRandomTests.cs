using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using System.Linq;
using System.Threading.Tasks;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsRandomTests
    {
        private readonly Mock<WireMockServerSettings> _settingsMock = new Mock<WireMockServerSettings>();
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Text = "{{Random Type=\"Text\" Min=8 Max=20}}",
                    DateTime = "{{Random Type=\"DateTime\"}}",
                    Integer = "{{Random Type=\"Integer\" Min=1000 Max=1000}}",
                    Long = "{{Random Type=\"Long\" Min=77777777 Max=99999999}}"
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Text"].Value<string>()).IsNotEmpty();
            Check.That(j["Integer"].Value<int>()).IsEqualTo(1000);
            Check.That(j["Long"].Value<long>()).IsStrictlyGreaterThan(77777777).And.IsStrictlyLessThan(99999999);
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1_Boolean()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Value = "{{Random Type=\"Boolean\"}}"
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Value"].Type).IsEqualTo(JTokenType.Boolean);
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1_Guid()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Guid1 = "{{Random Type=\"Guid\" Uppercase=false}}",
                    Guid2 = "{{Random Type=\"Guid\"}}"
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            string guid1 = j["Guid1"].Value<string>();
            Check.That(guid1.ToUpper()).IsNotEqualTo(guid1);
            string guid2 = j["Guid2"].Value<string>();
            Check.That(guid2.ToUpper()).IsEqualTo(guid2);
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1_StringList()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    StringValue = "{{Random Type=\"StringList\" Values=[\"a\", \"b\", \"c\"]}}"
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            string value = j["StringValue"].Value<string>();
            Check.That(new[] { "a", "b", "c" }.Contains(value)).IsTrue();
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random2()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Integer = "{{#Random Type=\"Integer\" Min=10000000 Max=99999999}}{{this}}{{/Random}}",
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Integer"].Value<int>()).IsStrictlyGreaterThan(10000000).And.IsStrictlyLessThan(99999999);
        }
    }
}
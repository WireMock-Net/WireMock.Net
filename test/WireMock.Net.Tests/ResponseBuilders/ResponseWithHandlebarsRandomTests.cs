using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsRandomTests
    {
        private const string ClientIp = "::1";

        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        public ResponseWithHandlebarsRandomTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Text = "{{Random Type=\"Text\" Min=8 Max=20}}",
                    DateTime = "{{Random Type=\"DateTime\"}}",
                    Integer = "{{Random Type=\"Integer\" Min=1000 Max=1000}}",
                    Long = "{{Random Type=\"Long\" Min=77777777 Max=99999999}}"
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["Text"].Value<string>()).IsNotEmpty();
            Check.That(j["Integer"].Value<int>()).IsEqualTo(1000);
            Check.That(j["Long"].Value<long>()).IsStrictlyGreaterThan(77777777).And.IsStrictlyLessThan(99999999);
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1_Boolean()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Value = "{{Random Type=\"Boolean\"}}"
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["Value"].Type).IsEqualTo(JTokenType.Boolean);
        }

        [Theory]
        [InlineData(ReplaceNodeOption.None, JTokenType.String)]
        [InlineData(ReplaceNodeOption.Bool, JTokenType.String)]
        [InlineData(ReplaceNodeOption.Integer, JTokenType.Integer)]
        [InlineData(ReplaceNodeOption.Bool | ReplaceNodeOption.Integer, JTokenType.Integer)]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1_Integer(ReplaceNodeOption option, JTokenType expected)
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Value = "{{Random Type=\"Integer\"}}"
                })
                .WithTransformer(option);

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["Value"].Type).IsEqualTo(expected);
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random1_Guid()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Guid1 = "{{Random Type=\"Guid\" Uppercase=false}}",
                    Guid2 = "{{Random Type=\"Guid\"}}"
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
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

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    StringValue = "{{Random Type=\"StringList\" Values=[\"a\", \"b\", \"c\"]}}"
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            string value = j["StringValue"].Value<string>();
            Check.That(new[] { "a", "b", "c" }.Contains(value)).IsTrue();
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_Random2()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var responseBuilder = Response.Create()
                .WithBodyAsJson(new
                {
                    Integer = "{{#Random Type=\"Integer\" Min=10000000 Max=99999999}}{{this}}{{/Random}}",
                })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["Integer"].Value<int>()).IsStrictlyGreaterThan(10000000).And.IsStrictlyLessThan(99999999);
        }
    }
}
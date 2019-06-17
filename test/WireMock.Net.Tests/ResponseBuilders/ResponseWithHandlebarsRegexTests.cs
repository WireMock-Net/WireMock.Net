using Moq;
using NFluent;
using System;
using System.Threading.Tasks;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsRegexTests
    {
        private readonly Mock<IFluentMockServerSettings> _settingsMock = new Mock<IFluentMockServerSettings>();
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{Regex.Match request.body \"^(?<x>\\w+)$\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("abc");
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch_NoMatch()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{Regex.Match request.body \"^?0$\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("");
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch_NoMatch_WithDefaultValue()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{Regex.Match request.body \"^?0$\" \"d\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("d");
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch2()
        {
            // Assign
            var body = new BodyData { BodyAsString = "https://localhost:5000/", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{this.port}}-{{this.proto}}{{/Regex.Match}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("5000-https");
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch2_NoMatch()
        {
            // Assign
            var body = new BodyData { BodyAsString = "{{\\test", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{this}}{{/Regex.Match}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("");
        }

        [Fact]
        public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch2_NoMatch_WithDefaultValue()
        {
            // Assign
            var body = new BodyData { BodyAsString = "{{\\test", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\" \"x\"}}{{this}}{{/Regex.Match}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settingsMock.Object);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("x");
        }

        [Fact]
        public void Response_ProvideResponseAsync_Handlebars_RegexMatch2_Throws()
        {
            // Assign
            var body = new BodyData { BodyAsString = "{{\\test", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.bodyAsJson \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{/Regex.Match}}")
                .WithTransformer();

            // Act and Assert
            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request, _settingsMock.Object)).Throws<NotSupportedException>();
        }
    }
}
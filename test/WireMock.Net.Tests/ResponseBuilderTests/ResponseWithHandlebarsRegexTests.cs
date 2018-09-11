using System;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilderTests
{
    public class ResponseWithHandlebarsRegexTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async void Response_ProvideResponse_Handlebars_RegexMatch()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{Regex.Match request.body \"^(?<x>\\w+)$\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // assert
            Check.That(responseMessage.Body).Equals("abc");
        }

        [Fact]
        public async void Response_ProvideResponse_Handlebars_RegexMatch_NoMatch()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{Regex.Match request.body \"^?0$\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // assert
            Check.That(responseMessage.Body).Equals("");
        }

        [Fact]
        public async void Response_ProvideResponse_Handlebars_RegexMatch_NoMatch_WithDefaultValue()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{Regex.Match request.body \"^?0$\" \"d\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // assert
            Check.That(responseMessage.Body).Equals("d");
        }

        [Fact]
        public async void Response_ProvideResponse_Handlebars_RegexMatch2()
        {
            // Assign
            var body = new BodyData { BodyAsString = "https://localhost:5000/" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{this.port}}-{{this.proto}}{{/Regex.Match}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // assert
            Check.That(responseMessage.Body).Equals("5000-https");
        }

        [Fact]
        public async void Response_ProvideResponse_Handlebars_RegexMatch2_NoMatch()
        {
            // Assign
            var body = new BodyData { BodyAsString = "{{\\test" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{this}}{{/Regex.Match}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // assert
            Check.That(responseMessage.Body).Equals("");
        }

        [Fact]
        public async void Response_ProvideResponse_Handlebars_RegexMatch2_NoMatch_WithDefaultValue()
        {
            // Assign
            var body = new BodyData { BodyAsString = "{{\\test" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\" \"x\"}}{{this}}{{/Regex.Match}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // assert
            Check.That(responseMessage.Body).Equals("x");
        }

        [Fact]
        public void Response_ProvideResponse_Handlebars_RegexMatch2_Throws()
        {
            // Assign
            var body = new BodyData { BodyAsString = "{{\\test" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{#Regex.Match request.bodyAsJson \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{/Regex.Match}}")
                .WithTransformer();

            // Act and Assert
            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request)).Throws<NotSupportedException>();
        }
    }
}
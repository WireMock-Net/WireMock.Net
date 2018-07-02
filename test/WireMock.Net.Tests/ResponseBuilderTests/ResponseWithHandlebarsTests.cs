using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilderTests
{
    public class ResponseWithHandlebarsTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_WithBodyAsJson()
        {
            // given
            string jsonString = "{ \"things\": [ { \"name\": \"RequiredThing\" }, { \"name\": \"Wiremock\" } ] }";
            var bodyData = new BodyData
            {
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                Encoding = Encoding.UTF8
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, bodyData);

            var response = Response.Create()
                .WithBodyAsJson(new { x = "test {{request.url}}" })
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(JsonConvert.SerializeObject(responseMessage.BodyAsJson)).Equals("{\"x\":\"test http://localhost/foo\"}");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_UrlPathVerb()
        {
            // given
            var body = new BodyData
            {
                BodyAsString = "whatever"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("test {{request.url}} {{request.path}} {{request.method}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test http://localhost/foo /foo post");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_UrlPath()
        {
            // given
            var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
            var request = new RequestMessage(urlDetails, "POST", ClientIp);

            var response = Response.Create()
                .WithBody("{{request.url}} {{request.absoluteurl}} {{request.path}} {{request.absolutepath}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("http://localhost/a/b http://localhost/wiremock/a/b /a/b /wiremock/a/b");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_PathSegments()
        {
            // given
            var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
            var request = new RequestMessage(urlDetails, "POST", ClientIp);

            var response = Response.Create()
                .WithBody("{{request.pathsegments.[0]}} {{request.absolutepathsegments.[0]}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("a wiremock");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Query()
        {
            // given
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo?a=1&a=2&b=5"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("test keya={{request.query.a}} idx={{request.query.a.[0]}} idx={{request.query.a.[1]}} keyb={{request.query.b}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test keya=1 idx=1 idx=2 keyb=5");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Header()
        {
            // given
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, new Dictionary<string, string[]> { { "Content-Type", new[] { "text/plain" } } });

            var response = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}").WithBody("test").WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test");
            Check.That(responseMessage.Headers).ContainsKey("x");
            Check.That(responseMessage.Headers["x"]).ContainsExactly("text/plain");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Headers()
        {
            // given
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, new Dictionary<string, string[]> { { "Content-Type", new[] { "text/plain" } } });

            var response = Response.Create().WithHeader("x", "{{request.headers.Content-Type}}", "{{request.url}}").WithBody("test").WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test");
            Check.That(responseMessage.Headers).ContainsKey("x");
            Check.That(responseMessage.Headers["x"]).Contains("text/plain");
            Check.That(responseMessage.Headers["x"]).Contains("http://localhost/foo");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Origin_Port_Protocol_Host()
        {
            // given
            var body = new BodyData
            {
                BodyAsString = "abc"
            };
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("test {{request.origin}} {{request.port}} {{request.protocol}} {{request.host}}")
                .WithTransformer();

            // act
            var responseMessage = await response.ProvideResponseAsync(request);

            // then
            Check.That(responseMessage.Body).Equals("test http://localhost:1234 1234 http localhost");
        }
    }
}
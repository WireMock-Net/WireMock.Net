using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilderTests
{
    public class ResponseWithHandlebarsLinqTests
    {
        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Linq1_String0()
        {
            // Assign
            var body = new BodyData { };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234/pathtest"), "POST", "::1", body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{Linq request.Path 'it'}}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"]).IsNotNull();
            Check.That(j["x"].ToString()).Equals("/pathtest");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Linq1_String1()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new JObject
                {
                    { "Id", new JValue(9) },
                    { "Name", new JValue("Test") }
                }
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson 'it.Name + \"_123\"' }}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"]).IsNotNull();
            Check.That(j["x"].ToString()).Equals("Test_123");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Linq1_String2()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new JObject
                {
                    { "Id", new JValue(9) },
                    { "Name", new JValue("Test") }
                }
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson 'new(it.Name + \"_123\" as N, it.Id as I)' }}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"]).IsNotNull();
            Check.That(j["x"].ToString()).Equals("{ N = Test_123, I = 9 }");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_Linq2_Object()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new JObject
                {
                    { "Id", new JValue(9) },
                    { "Name", new JValue("Test") }
                }
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{#Linq request.bodyAsJson 'new(it.Name + \"_123\" as N, it.Id as I)' }}{{this}}{{/Linq}}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"]).IsNotNull();
            Check.That(j["x"].ToString()).Equals("{ N = Test_123, I = 9 }");
        }

        [Fact]
        public void Response_ProvideResponse_Handlebars_Linq_Throws_NotSupportedException()
        {
            // Assign
            var body = new BodyData { BodyAsJson = new { x = "x" }};

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson 1}}" })
                .WithTransformer();

            // Act
            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request)).Throws<NotSupportedException>();
        }

        [Fact]
        public void Response_ProvideResponse_Handlebars_Linq1_Throws_ArgumentNullException()
        {
            // Assign
            var body = new BodyData { };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithBodyAsJson(new { x = "{{Linq request.body 'Name'}}" })
                .WithTransformer();

            // Act
            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request)).Throws<ArgumentNullException>();
        }

        [Fact]
        public void Response_ProvideResponse_Handlebars_Linq1_Throws_ArgumentException()
        {
            // Assign
            var body = new BodyData { };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson}} ''" })
                .WithTransformer();

            // Act
            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request)).Throws<ArgumentException>();
        }

        [Fact]
        public async void Response_ProvideResponse_Handlebars_Linq1_ParseError_Returns_Empty()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new JObject
                {
                    { "Id", new JValue(9) },
                    { "Name", new JValue("Test") }
                }
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithBodyAsJson(new { x = "{{Linq request.bodyAsJson '---' }}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"].ToString()).IsEmpty();
        }

        [Fact]
        public async void Response_ProvideResponse_Handlebars_Linq2_ParseError_Returns_Empty()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = new JObject
                {
                    { "Id", new JValue(9) },
                    { "Name", new JValue("Test") }
                }
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", "::1", body);

            var response = Response.Create()
                .WithBodyAsJson(new { x = "{{#Linq request.bodyAsJson '---' }}{{this}}{{/Linq}}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"].ToString()).IsEmpty();
        }
    }
}
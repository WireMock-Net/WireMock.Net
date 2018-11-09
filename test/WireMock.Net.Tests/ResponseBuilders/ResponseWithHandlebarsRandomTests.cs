using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsRandomTests
    {
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
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Text"]).IsNotNull();
            Check.That(j["Integer"].Value<int>()).IsEqualTo(1000);
            Check.That(j["Long"].Value<int>()).IsStrictlyGreaterThan(77777777).And.IsStrictlyLessThan(99999999);
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
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            string guid1 = j["Guid1"].Value<string>();
            Check.That(guid1.ToUpper()).IsNotEqualTo(guid1);
            string guid2 = j["Guid2"].Value<string>();
            Check.That(guid2.ToUpper()).IsEqualTo(guid2);
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
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyData.BodyAsJson);
            Check.That(j["Integer"].Value<int>()).IsStrictlyGreaterThan(10000000).And.IsStrictlyLessThan(99999999);
        }
    }
}
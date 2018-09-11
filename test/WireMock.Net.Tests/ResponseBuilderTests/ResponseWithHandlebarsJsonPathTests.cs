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
    public class ResponseWithHandlebarsJsonPathTests
    {
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Object_ResponseBodyAsJson()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = @"{
                  ""Stores"": [
                    ""Lambton Quay"",
                    ""Willis Street""
                  ],
                  ""Manufacturers"": [
                    {
                      ""Name"": ""Acme Co"",
                      ""Products"": [
                        {
                          ""Name"": ""Anvil"",
                          ""Price"": 50
                        }
                      ]
                    },
                    {
                      ""Name"": ""Contoso"",
                      ""Products"": [
                        {
                          ""Name"": ""Elbow Grease"",
                          ""Price"": 99.95
                        },
                        {
                          ""Name"": ""Headlight Fluid"",
                          ""Price"": 4
                        }
                      ]
                    }
                  ]
                }"
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"]).IsNotNull();
            Check.That(j["x"]["Name"].ToString()).Equals("Acme Co");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Number_ResponseBodyAsJson()
        {
            // Assign
            var body = new BodyData { BodyAsString = "{ \"Price\": 99 }" };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{JsonPath.SelectToken request.body \"..Price\"}}" })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            JObject j = JObject.FromObject(responseMessage.BodyAsJson);
            Check.That(j["x"].Value<long>()).Equals(99);
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Request_BodyAsString()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = @"{
                  ""Stores"": [
                    ""Lambton Quay"",
                    ""Willis Street""
                  ],
                  ""Manufacturers"": [
                    {
                      ""Name"": ""Acme Co"",
                      ""Products"": [
                        {
                          ""Name"": ""Anvil"",
                          ""Price"": 50
                        }
                      ]
                    },
                    {
                      ""Name"": ""Contoso"",
                      ""Products"": [
                        {
                          ""Name"": ""Elbow Grease"",
                          ""Price"": 99.95
                        },
                        {
                          ""Name"": ""Headlight Fluid"",
                          ""Price"": 4
                        }
                      ]
                    }
                  ]
                }"
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Body).Equals($"{{{Environment.NewLine}  \"Name\": \"Acme Co\",{Environment.NewLine}  \"Products\": [{Environment.NewLine}    {{{Environment.NewLine}      \"Name\": \"Anvil\",{Environment.NewLine}      \"Price\": 50{Environment.NewLine}    }}{Environment.NewLine}  ]{Environment.NewLine}}}");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Request_BodyAsJObject()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = JObject.Parse(@"{
                  'Stores': [
                    'Lambton Quay',
                    'Willis Street'
                  ],
                  'Manufacturers': [
                    {
                      'Name': 'Acme Co',
                      'Products': [
                        {
                          'Name': 'Anvil',
                          'Price': 50
                        }
                      ]
                    },
                    {
                      'Name': 'Contoso',
                      'Products': [
                        {
                          'Name': 'Elbow Grease',
                          'Price': 99.95
                        },
                        {
                          'Name': 'Headlight Fluid',
                          'Price': 4
                        }
                      ]
                    }
                  ]
                }")
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{JsonPath.SelectToken request.bodyAsJson \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Body).Equals($"{{{Environment.NewLine}  \"Name\": \"Acme Co\",{Environment.NewLine}  \"Products\": [{Environment.NewLine}    {{{Environment.NewLine}      \"Name\": \"Anvil\",{Environment.NewLine}      \"Price\": 50{Environment.NewLine}    }}{Environment.NewLine}  ]{Environment.NewLine}}}");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectTokens_Request_BodyAsString()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = @"{
                  ""Stores"": [
                    ""Lambton Quay"",
                    ""Willis Street""
                  ],
                  ""Manufacturers"": [
                    {
                      ""Name"": ""Acme Co"",
                      ""Products"": [
                        {
                          ""Name"": ""Anvil"",
                          ""Price"": 50
                        }
                      ]
                    },
                    {
                      ""Name"": ""Contoso"",
                      ""Products"": [
                        {
                          ""Name"": ""Elbow Grease"",
                          ""Price"": 99.95
                        },
                        {
                          ""Name"": ""Headlight Fluid"",
                          ""Price"": 4
                        }
                      ]
                    }
                  ]
                }"
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{#JsonPath.SelectTokens request.body \"$..Products[?(@.Price >= 50)].Name\"}}{{#each this}}%{{@index}}:{{this}}%{{/each}}{{/JsonPath.SelectTokens}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Body).Equals("%0:Anvil%%1:Elbow Grease%");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectTokens_Request_BodyAsJObject()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = JObject.Parse(@"{
                  'Stores': [
                    'Lambton Quay',
                    'Willis Street'
                  ],
                  'Manufacturers': [
                    {
                      'Name': 'Acme Co',
                      'Products': [
                        {
                          'Name': 'Anvil',
                          'Price': 50
                        }
                      ]
                    },
                    {
                      'Name': 'Contoso',
                      'Products': [
                        {
                          'Name': 'Elbow Grease',
                          'Price': 99.95
                        },
                        {
                          'Name': 'Headlight Fluid',
                          'Price': 4
                        }
                      ]
                    }
                  ]
                }")
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{#JsonPath.SelectTokens request.bodyAsJson \"$..Products[?(@.Price >= 50)].Name\"}}{{#each this}}%{{@index}}:{{this}}%{{/each}}{{/JsonPath.SelectTokens}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Body).Equals("%0:Anvil%%1:Elbow Grease%");
        }

        [Fact]
        public void Response_ProvideResponse_Handlebars_JsonPath_SelectTokens_Throws()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsJson = JObject.Parse(@"{
                  'Stores': [
                    'Lambton Quay',
                    'Willis Street'
                  ]
                }")
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{#JsonPath.SelectTokens request.body \"$..Products[?(@.Price >= 50)].Name\"}}{{id}} {{value}},{{/JsonPath.SelectTokens}}")
                .WithTransformer();

            // Act
            Check.ThatAsyncCode(() => response.ProvideResponseAsync(request)).Throws<ArgumentNullException>();
        }
    }
}
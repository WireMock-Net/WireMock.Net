using System;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsJsonPathTests
    {
        private const string ClientIp = "::1";

        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        public ResponseWithHandlebarsJsonPathTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

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
                }",
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}" })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
            Check.That(j["x"]).IsNotNull();
            Check.That(j["x"]["Name"].ToString()).Equals("Acme Co");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_JsonPath_SelectToken_Number_ResponseBodyAsJson()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = "{ \"Price\": 99 }",
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { x = "{{JsonPath.SelectToken request.body \"..Price\"}}" })
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
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
                }",
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{JsonPath.SelectToken request.body \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsString).Equals($"{{{Environment.NewLine}  \"Name\": \"Acme Co\",{Environment.NewLine}  \"Products\": [{Environment.NewLine}    {{{Environment.NewLine}      \"Name\": \"Anvil\",{Environment.NewLine}      \"Price\": 50{Environment.NewLine}    }}{Environment.NewLine}  ]{Environment.NewLine}}}");
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
                }"),
                DetectedBodyType = BodyType.Json
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{JsonPath.SelectToken request.bodyAsJson \"$.Manufacturers[?(@.Name == 'Acme Co')]\"}}")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsString).Equals($"{{{Environment.NewLine}  \"Name\": \"Acme Co\",{Environment.NewLine}  \"Products\": [{Environment.NewLine}    {{{Environment.NewLine}      \"Name\": \"Anvil\",{Environment.NewLine}      \"Price\": 50{Environment.NewLine}    }}{Environment.NewLine}  ]{Environment.NewLine}}}");
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
                }",
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{#JsonPath.SelectTokens request.body \"$..Products[?(@.Price >= 50)].Name\"}}{{#each this}}%{{@index}}:{{this}}%{{/each}}{{/JsonPath.SelectTokens}}")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsString).Equals("%0:Anvil%%1:Elbow Grease%");
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
                }"),
                DetectedBodyType = BodyType.Json
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{#JsonPath.SelectTokens request.bodyAsJson \"$..Products[?(@.Price >= 50)].Name\"}}{{#each this}}%{{@index}}:{{this}}%{{/each}}{{/JsonPath.SelectTokens}}")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsString).Equals("%0:Anvil%%1:Elbow Grease%");
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
                }"),
                DetectedBodyType = BodyType.Json
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBody("{{#JsonPath.SelectTokens request.body \"$..Products[?(@.Price >= 50)].Name\"}}{{id}} {{value}},{{/JsonPath.SelectTokens}}")
                .WithTransformer();

            // Act
            Check.ThatAsyncCode(() => responseBuilder.ProvideResponseAsync(request, _settings)).Throws<ArgumentNullException>();
        }

        [Fact]
        public async Task Response_ProvideResponse_Transformer_WithBodyAsFile_JsonPath()
        {
            // Assign
            string jsonString = "{ \"MyUniqueNumber\": \"1\" }";
            var bodyData = new BodyData
            {
                BodyAsString = jsonString,
                BodyAsJson = JsonConvert.DeserializeObject(jsonString),
                DetectedBodyType = BodyType.Json,
                DetectedBodyTypeFromContentType = BodyType.Json,
                Encoding = Encoding.UTF8
            };
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, bodyData);

            string jsonPath = "\"$.MyUniqueNumber\"";
            var responseBuilder = Response.Create()
                .WithTransformer()
                .WithBodyFromFile(@"c:\\{{JsonPath.SelectToken request.body " + jsonPath + "}}\\test.json"); // why use a \\ here ?

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsFile).Equals(@"c:\1\test.json");
        }
    }
}
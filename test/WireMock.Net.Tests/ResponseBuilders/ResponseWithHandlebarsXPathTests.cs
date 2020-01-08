using System.Threading.Tasks;
using System.Xml;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;
#if !NETSTANDARD1_3
using Wmhelp.XPath2;
#endif

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsXPathTests
    {
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_XPath_SelectSingleNode_Request_BodyAsString()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("<response>{{XPath.SelectSingleNode request.body \"/todo-list/todo-item[1]\"}}</response>")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            var nav = new XmlDocument { InnerXml = responseMessage.BodyData.BodyAsString }.CreateNavigator();
            var node = nav.XPath2SelectSingleNode("/response/todo-item");
            Check.That(node.Value).Equals("abc");
            Check.That(node.GetAttribute("id", "")).Equals("a1");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_XPath_SelectNodes_Request_BodyAsString()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("<response>{{XPath.SelectNodes request.body \"/todo-list/todo-item\"}}</response>")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            var nav = new XmlDocument { InnerXml = responseMessage.BodyData.BodyAsString }.CreateNavigator();
            var nodes = nav.XPath2SelectNodes("/response/todo-item");
            Check.That(nodes.Count + 1).IsEqualTo(3);
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_XPath_Evaluate_Request_BodyAsString()
        {
            // Assign
            var body = new BodyData
            {
                BodyAsString = @"<todo-list>
                                   <todo-item id='a1'>abc</todo-item>
                                   <todo-item id='a2'>def</todo-item>
                                   <todo-item id='a3'>xyz</todo-item>
                                 </todo-list>",
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("{{XPath.Evaluate request.body \"boolean(/todo-list[count(todo-item) = 3])\"}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // Assert
            Check.That(responseMessage.BodyData.BodyAsString).IsEqualIgnoringCase("True");
        }
    }
}

using System.Threading.Tasks;
using System.Xml;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;
using Moq;
using WireMock.Handlers;
using FluentAssertions;
#if !NETSTANDARD1_3
using Wmhelp.XPath2;
#endif

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsXPathTests
    {
        private const string ClientIp = "::1";

        private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        public ResponseWithHandlebarsXPathTests()
        {
            _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
            _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

            _settings.FileSystemHandler = _filesystemHandlerMock.Object;
        }

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

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("<response>{{XPath.SelectSingleNode request.body \"/todo-list/todo-item[1]\"}}</response>")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            var nav = new XmlDocument { InnerXml = response.Message.BodyData.BodyAsString }.CreateNavigator();
            var node = nav.XPath2SelectSingleNode("/response/todo-item");
            Check.That(node.Value).Equals("abc");
            Check.That(node.GetAttribute("id", "")).Equals("a1");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_XPath_SelectSingleNode_Text_Request_BodyAsString()
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

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("{{XPath.SelectSingleNode request.body \"/todo-list/todo-item[1]/text()\"}}")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsString).IsEqualTo("abc");
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

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("<response>{{XPath.SelectNodes request.body \"/todo-list/todo-item\"}}</response>")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            var nav = new XmlDocument { InnerXml = response.Message.BodyData.BodyAsString }.CreateNavigator();
            var nodes = nav.XPath2SelectNodes("/response/todo-item");
            Check.That(nodes.Count + 1).IsEqualTo(3);
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_XPath_SelectSingleNode_Request_SoapXML_BodyAsString()
        {
            // Assign
            string soap = @"
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://www.Test.nl/XMLHeader/10"" xmlns:req=""http://www.Test.nl/Betalen/COU/Services/RdplDbTknLystByOvkLyst/8/Req"">
   <soapenv:Header>
      <ns:TestHeader>
         <ns:HeaderVersion>10</ns:HeaderVersion>
         <ns:MessageId>MsgId10</ns:MessageId>
         <ns:ServiceRequestorDomain>Betalen</ns:ServiceRequestorDomain>
         <ns:ServiceRequestorId>CRM</ns:ServiceRequestorId>
         <ns:ServiceProviderDomain>COU</ns:ServiceProviderDomain>
         <ns:ServiceId>RdplDbTknLystByOvkLyst</ns:ServiceId>
         <ns:ServiceVersion>8</ns:ServiceVersion>
         <ns:FaultIndication>N</ns:FaultIndication>
         <ns:MessageTimestamp>?</ns:MessageTimestamp>
      </ns:TestHeader>
   </soapenv:Header>
   <soapenv:Body>
      <req:RdplDbTknLystByOvkLyst_REQ>
         <req:AanleveraarCode>CRM</req:AanleveraarCode>
         <!--Optional:-->
         <req:AanleveraarDetail>CRMi</req:AanleveraarDetail>
         <req:BerichtId>BerId</req:BerichtId>
         <req:BerichtType>RdplDbTknLystByOvkLyst</req:BerichtType>
         <!--Optional:-->
         <req:OpgenomenBedragenGewenstIndicatie>N</req:OpgenomenBedragenGewenstIndicatie>
         <req:TokenIdLijst>
            <!--1 to 10 repetitions:-->
            <req:TokenId>0000083256</req:TokenId>
            <req:TokenId>0000083259</req:TokenId>
         </req:TokenIdLijst>
      </req:RdplDbTknLystByOvkLyst_REQ>
   </soapenv:Body>
</soapenv:Envelope>";
            var body = new BodyData
            {
                BodyAsString = soap,
                DetectedBodyType = BodyType.String
            };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("<response>{{XPath.SelectSingleNode request.body \"//*[local-name()='TokenIdLijst']\"}}</response>")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            response.Message.BodyData.BodyAsString.Should().Contain("TokenIdLijst").And.Contain("0000083256").And.Contain("0000083259");
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

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("{{XPath.Evaluate request.body \"boolean(/todo-list[count(todo-item) = 3])\"}}")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsString).IsEqualIgnoringCase("True");
        }

        [Fact]
        public async Task Response_ProvideResponse_Handlebars_XPath_Evaluate_Attribute_Request_BodyAsString()
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

            var responseBuilder = Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody("{{XPath.Evaluate request.body \"string(/todo-list/todo-item[1]/@id)\"}}")
                .WithTransformer();

            // Act
            var response = await responseBuilder.ProvideResponseAsync(request, _settings).ConfigureAwait(false);

            // Assert
            Check.That(response.Message.BodyData.BodyAsString).IsEqualTo("a1");
        }
    }
}

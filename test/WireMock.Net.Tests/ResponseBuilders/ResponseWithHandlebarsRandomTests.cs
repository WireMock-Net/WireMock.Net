using System.Threading.Tasks;
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
        public async Task Response_ProvideResponseAsync_Handlebars_Random()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

            var response = Response.Create()
                .WithBodyAsJson(new
                {
                    Text = "{{Random Type=\"Text\" Min=8 Max=20}}",
                    DateTime = "{{Random Type=\"DateTime\"}}",
                    Guid = "{{Random Type=\"Guid\" Uppercase=true}}"
                })
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // assert
            Check.That(responseMessage.BodyData).IsNotNull();
        }
    }
}
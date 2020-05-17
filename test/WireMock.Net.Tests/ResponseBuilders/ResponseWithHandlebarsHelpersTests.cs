using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithHandlebarsHelpersTests
    {
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();
        private const string ClientIp = "::1";

        [Fact]
        public async Task Response_ProvideResponseAsync_HandlebarsHelpers_String_Uppercase()
        {
            // Assign
            var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

            var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

            var response = Response.Create()
                .WithBody("{{String.Uppercase request.body}}")
                .WithTransformer();

            // Act
            var responseMessage = await response.ProvideResponseAsync(request, _settings);

            // assert
            Check.That(responseMessage.BodyData.BodyAsString).Equals("ABC");
        }
    }
}
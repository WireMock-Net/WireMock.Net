using System.Threading.Tasks;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders
{
    public class ResponseWithCallbackTests
    {
        [Fact]
        public async Task Response_WithCallback()
        {
            // Assign
            var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", "::1");
            var response = Response.Create().WithCallback(req => new ResponseMessage { Body = req.Path + "Bar", StatusCode = 302 });

            // Act
            var responseMessage = await response.ProvideResponseAsync(request);

            // Assert
            Check.That(responseMessage.Body).IsEqualTo("/fooBar");
            Check.That(responseMessage.StatusCode).IsEqualTo(302);
        }
    }
}
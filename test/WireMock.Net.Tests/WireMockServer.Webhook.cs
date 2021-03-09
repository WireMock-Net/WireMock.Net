using FluentAssertions;
using NFluent;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests
{
    public class WireMockServerWebhookTests
    {
        [Fact]
        public async Task WireMockServer_WithWebhook_Should_Send_Message_To_Webhook()
        {
            // Assign
            var serverReceivingTheWebhook = WireMockServer.Start();
            serverReceivingTheWebhook.Given(Request.Create().UsingPost()).RespondWith(Response.Create().WithStatusCode(200));

            // Act
            var server = WireMockServer.Start();
            server.Given(Request.Create().UsingPost())
                .WithWebhook(new Webhook
                {
                    Request = new WebhookRequest
                    {
                        Url = serverReceivingTheWebhook.Urls[0],
                        Method = "post",
                        BodyData = new BodyData
                        {
                            BodyAsString = "abc",
                            DetectedBodyType = BodyType.String,
                            DetectedBodyTypeFromContentType = BodyType.String
                        }
                    }
                })
                .RespondWith(Response.Create().WithBody("a-response"));

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{server.Urls[0]}/TST"),
                Content = new StringContent("test")
            };

            // Assert
            var response = await new HttpClient().SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Be("a-response");

            serverReceivingTheWebhook.LogEntries.Should().HaveCount(1);

            server.Dispose();
            serverReceivingTheWebhook.Dispose();
        }
    }
}
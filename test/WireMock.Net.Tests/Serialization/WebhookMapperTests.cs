using System.Collections.Generic;
using FluentAssertions;
using WireMock.Admin.Mappings;
using WireMock.Serialization;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class WebhookMapperTests
    {
        [Fact]
        public void WebhookMapper_Map_Model_BodyAsString_And_UseTransformerIsFalse()
        {
            // Assign
            var model = new WebhookModel
            {
                Request = new WebhookRequestModel
                {
                    Url = "https://localhost",
                    Method = "get",
                    Headers = new Dictionary<string, string>
                    {
                        { "x", "y" }
                    },
                    Body = "test",
                    UseTransformer = false
                }
            };

            var result = WebhookMapper.Map(model);

            result.Request.Url.Should().Be("https://localhost");
            result.Request.Method.Should().Be("get");
            result.Request.Headers.Should().HaveCount(1);
            result.Request.BodyData.BodyAsJson.Should().BeNull();
            result.Request.BodyData.BodyAsString.Should().Be("test");
            result.Request.BodyData.DetectedBodyType.Should().Be(BodyType.String);
            result.Request.UseTransformer.Should().BeNull();
        }

        [Fact]
        public void WebhookMapper_Map_Model_BodyAsString_And_UseTransformerIsTrue()
        {
            // Assign
            var model = new WebhookModel
            {
                Request = new WebhookRequestModel
                {
                    Url = "https://localhost",
                    Method = "get",
                    Headers = new Dictionary<string, string>
                    {
                        { "x", "y" }
                    },
                    Body = "test",
                    UseTransformer = true
                }
            };

            var result = WebhookMapper.Map(model);

            result.Request.Url.Should().Be("https://localhost");
            result.Request.Method.Should().Be("get");
            result.Request.Headers.Should().HaveCount(1);
            result.Request.BodyData.BodyAsJson.Should().BeNull();
            result.Request.BodyData.BodyAsString.Should().Be("test");
            result.Request.BodyData.DetectedBodyType.Should().Be(BodyType.String);
            result.Request.UseTransformer.Should().BeTrue();
            result.Request.TransformerType.Should().Be(TransformerType.Handlebars);
        }

        [Fact]
        public void WebhookMapper_Map_Model_BodyAsJson()
        {
            // Assign
            var model = new WebhookModel
            {
                Request = new WebhookRequestModel
                {
                    Url = "https://localhost",
                    Method = "get",
                    Headers = new Dictionary<string, string>
                    {
                        { "x", "y" }
                    },
                    BodyAsJson = new { n = 12345 }
                }
            };

            var result = WebhookMapper.Map(model);

            result.Request.Url.Should().Be("https://localhost");
            result.Request.Method.Should().Be("get");
            result.Request.Headers.Should().HaveCount(1);
            result.Request.BodyData.BodyAsString.Should().BeNull();
            result.Request.BodyData.BodyAsJson.Should().NotBeNull();
            result.Request.BodyData.DetectedBodyType.Should().Be(BodyType.Json);
        }
    }
}

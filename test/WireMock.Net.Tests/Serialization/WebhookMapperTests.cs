using System.Collections.Generic;
using FluentAssertions;
using WireMock.Admin.Mappings;
using WireMock.Models;
using WireMock.Serialization;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public class WebhookMapperTests
{
    [Fact]
    public void WebhookMapper_Map_WebhookModel_BodyAsString_And_UseTransformerIsFalse()
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
        result.Request.BodyData!.BodyAsJson.Should().BeNull();
        result.Request.BodyData.BodyAsString.Should().Be("test");
        result.Request.BodyData.DetectedBodyType.Should().Be(BodyType.String);
        result.Request.UseTransformer.Should().BeNull();
    }

    [Fact]
    public void WebhookMapper_Map_WebhookModel_BodyAsString_And_UseTransformerIsTrue()
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
        result.Request.BodyData!.BodyAsJson.Should().BeNull();
        result.Request.BodyData.BodyAsString.Should().Be("test");
        result.Request.BodyData.DetectedBodyType.Should().Be(BodyType.String);
        result.Request.UseTransformer.Should().BeTrue();
        result.Request.TransformerType.Should().Be(TransformerType.Handlebars);
    }

    [Fact]
    public void WebhookMapper_Map_WebhookModel_BodyAsJson()
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
                BodyAsJson = new { n = 12345 },
                UseFireAndForget = true
            }
        };

        var result = WebhookMapper.Map(model);

        result.Request.Url.Should().Be("https://localhost");
        result.Request.Method.Should().Be("get");
        result.Request.Headers.Should().HaveCount(1);
        result.Request.BodyData!.BodyAsString.Should().BeNull();
        result.Request.BodyData.BodyAsJson.Should().NotBeNull();
        result.Request.BodyData.DetectedBodyType.Should().Be(BodyType.Json);
        result.Request.UseFireAndForget.Should().BeTrue();
    }

    [Fact]
    public void WebhookMapper_Map_Webhook_To_Model()
    {
        // Assign
        var webhook = new Webhook
        {
            Request = new WebhookRequest
            {
                Url = "https://localhost",
                Method = "get",
                Headers = new Dictionary<string, WireMockList<string>>
                {
                    { "x", new WireMockList<string>("y") }
                },
                BodyData = new BodyData
                {
                    BodyAsJson = new { n = 12345 },
                    DetectedBodyType = BodyType.Json,
                    DetectedBodyTypeFromContentType = BodyType.Json
                },
                UseFireAndForget = true
            }
        };

        var result = WebhookMapper.Map(webhook);

        result.Request.Url.Should().Be("https://localhost");
        result.Request.Method.Should().Be("get");
        result.Request.Headers.Should().HaveCount(1);
        result.Request.BodyAsJson.Should().NotBeNull();
        result.Request.UseFireAndForget.Should().BeTrue();
    }
}
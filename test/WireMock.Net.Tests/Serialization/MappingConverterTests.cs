using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class MappingConverterTests
    {
        private readonly WireMockServerSettings _settings = new WireMockServerSettings();

        private readonly MappingConverter _sut;

        public MappingConverterTests()
        {
            _sut = new MappingConverter(new MatcherMapper(_settings));
        }

        [Fact]
        public void ToMappingModel_With_SingleWebHook()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create();
            var webhooks = new IWebhook[]
            {
                new Webhook
                {
                    Request = new WebhookRequest
                    {
                        Url = "https://test.com",
                        Headers = new Dictionary<string, WireMockList<string>>
                        {
                            { "Single", new WireMockList<string>("x") },
                            { "Multi", new WireMockList<string>("a", "b") }
                        },
                        Method = "post",
                        BodyData = new BodyData
                        {
                            BodyAsString = "b",
                            DetectedBodyType = BodyType.String,
                            DetectedBodyTypeFromContentType = BodyType.String
                        }
                    }
                }
            };
            var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, null, _settings, request, response, 0, null, null, null, null, webhooks, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Priority.Should().BeNull();

            model.Response.BodyAsJsonIndented.Should().BeNull();
            model.Response.UseTransformer.Should().BeNull();
            model.Response.Headers.Should().BeNull();

            model.Webhooks.Should().BeNull();

            model.Webhook.Request.Method.Should().Be("post");
            model.Webhook.Request.Url.Should().Be("https://test.com");
            model.Webhook.Request.Headers.Should().HaveCount(2);
            model.Webhook.Request.Body.Should().Be("b");
            model.Webhook.Request.BodyAsJson.Should().BeNull();
        }

        [Fact]
        public void ToMappingModel_With_MultipleWebHooks()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create();
            var webhooks = new IWebhook[]
            {
                new Webhook
                {
                    Request = new WebhookRequest
                    {
                        Url = "https://test1.com",
                        Headers = new Dictionary<string, WireMockList<string>>
                        {
                            { "One", new WireMockList<string>("x") }
                        },
                        Method = "post",
                        BodyData = new BodyData
                        {
                            BodyAsString = "1",
                            DetectedBodyType = BodyType.String,
                            DetectedBodyTypeFromContentType = BodyType.String
                        }
                    }
                },

                new Webhook
                {
                    Request = new WebhookRequest
                    {
                        Url = "https://test2.com",
                        Headers = new Dictionary<string, WireMockList<string>>
                        {
                            { "First", new WireMockList<string>("x") },
                            { "Second", new WireMockList<string>("a", "b") }
                        },
                        Method = "post",
                        BodyData = new BodyData
                        {
                            BodyAsString = "2",
                            DetectedBodyType = BodyType.String,
                            DetectedBodyTypeFromContentType = BodyType.String
                        }
                    }
                }
            };
            var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, null, _settings, request, response, 0, null, null, null, null, webhooks, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Priority.Should().BeNull();

            model.Response.BodyAsJsonIndented.Should().BeNull();
            model.Response.UseTransformer.Should().BeNull();
            model.Response.Headers.Should().BeNull();

            model.Webhook.Should().BeNull();

            model.Webhooks[0].Request.Method.Should().Be("post");
            model.Webhooks[0].Request.Url.Should().Be("https://test1.com");
            model.Webhooks[0].Request.Headers.Should().HaveCount(1);
            model.Webhooks[0].Request.Body.Should().Be("1");

            model.Webhooks[1].Request.Method.Should().Be("post");
            model.Webhooks[1].Request.Url.Should().Be("https://test2.com");
            model.Webhooks[1].Request.Headers.Should().HaveCount(2);
            model.Webhooks[1].Request.Body.Should().Be("2");
        }

        [Fact]
        public void ToMappingModel_WithTitle_And_Description_ReturnsCorrectModel()
        {
            // Assign
            var title = "my-title";
            var description = "my-description";
            var request = Request.Create();
            var response = Response.Create();
            var mapping = new Mapping(Guid.NewGuid(), title, description, null, _settings, request, response, 0, null, null, null, null, null, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Title.Should().Be(title);
            model.Description.Should().Be(description);
        }

        [Fact]
        public void ToMappingModel_WithPriority_ReturnsPriority()
        {
            // Assign
            var request = Request.Create();
            var response = Response.Create().WithBodyAsJson(new { x = "x" }).WithTransformer();
            var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Priority.Should().Be(42);
            model.Response.UseTransformer.Should().BeTrue();
        }

        [Fact]
        public void ToMappingModel_WithTimeSettings_ReturnsCorrectTimeSettings()
        {
            // Assign
            var start = DateTime.Now;
            var ttl = 100;
            var end = start.AddSeconds(ttl);
            var request = Request.Create();
            var response = Response.Create();
            var timeSettings = new TimeSettings
            {
                Start = start,
                End = end,
                TTL = ttl
            };
            var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, timeSettings);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.TimeSettings.Should().NotBeNull();
            model.TimeSettings.Start.Should().Be(start);
            model.TimeSettings.End.Should().Be(end);
            model.TimeSettings.TTL.Should().Be(ttl);
        }

        [Fact]
        public void ToMappingModel_WithDelayAsTimeSpan_ReturnsCorrectModel()
        {
            // Arrange
            var tests = new[]
            {
                new { Delay = Timeout.InfiniteTimeSpan, Expected = (int) TimeSpan.MaxValue.TotalMilliseconds },
                new { Delay = TimeSpan.FromSeconds(1), Expected = 1000},
                new { Delay = TimeSpan.MaxValue, Expected = (int) TimeSpan.MaxValue.TotalMilliseconds }
            };

            foreach (var test in tests)
            {
                var request = Request.Create();
                var response = Response.Create().WithDelay(test.Delay);
                var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, string.Empty, _settings, request, response, 42, null, null, null, null, null, null);

                // Act
                var model = _sut.ToMappingModel(mapping);

                // Assert
                model.Should().NotBeNull();
                model.Response.Delay.Should().Be(test.Expected);
            }
        }

        [Fact]
        public void ToMappingModel_WithDelayAsMilleSeconds_ReturnsCorrectModel()
        {
            // Assign
            var delay = 1000;
            var request = Request.Create();
            var response = Response.Create().WithDelay(delay);
            var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Response.Delay.Should().Be(delay);
        }

        [Fact]
        public void ToMappingModel_WithRandomMinimumDelay_ReturnsCorrectModel()
        {
            // Assign
            int minimumDelay = 1000;
            var request = Request.Create();
            var response = Response.Create().WithRandomDelay(minimumDelay);
            var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Response.Delay.Should().BeNull();
            model.Response.MinimumRandomDelay.Should().Be(minimumDelay);
            model.Response.MaximumRandomDelay.Should().Be(60_000);
        }

        [Fact]
        public void ToMappingModel_WithRandomDelay_ReturnsCorrectModel()
        {
            // Assign
            int minimumDelay = 1000;
            int maximumDelay = 2000;
            var request = Request.Create();
            var response = Response.Create().WithRandomDelay(minimumDelay, maximumDelay);
            var mapping = new Mapping(Guid.NewGuid(), string.Empty, string.Empty, null, _settings, request, response, 42, null, null, null, null, null, null);

            // Act
            var model = _sut.ToMappingModel(mapping);

            // Assert
            model.Should().NotBeNull();
            model.Response.Delay.Should().BeNull();
            model.Response.MinimumRandomDelay.Should().Be(minimumDelay);
            model.Response.MaximumRandomDelay.Should().Be(maximumDelay);
        }
    }
}
using System;
using Moq;
using Newtonsoft.Json;
using System.IO;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public class ProxyMappingConverterTests
{
    private readonly WireMockServerSettings _settings = new();

    private readonly MappingConverter _mappingConverter;

    private readonly ProxyMappingConverter _sut;

    public ProxyMappingConverterTests()
    {
        var guidUtilsMock = new Mock<IGuidUtils>();
        guidUtilsMock.Setup(g => g.NewGuid()).Returns(Guid.Parse("ff55ac0a-fea9-4d7b-be74-5e483a2c1305"));

        _mappingConverter = new MappingConverter(new MatcherMapper(_settings));

        _sut = new ProxyMappingConverter(_settings, guidUtilsMock.Object);
    }

    [Fact]
    public void ToMapping_UseDefinedRequestMatchers_True()
    {
        // Arrange
        var proxyAndRecordSettings = new ProxyAndRecordSettings
        {
            UseDefinedRequestMatchers = true
        };

        var request = Request.Create()
            .UsingPost()
            .WithPath("x")
            .WithParam("p1", "p1-v")
            .WithParam("p2", "p2-v")
            .WithHeader("Content-Type", new ContentTypeMatcher("text/plain"))
            .WithCookie("c", "x")
            .WithBody(new RegexMatcher("<RequestType>Auth</RequestType"));

        var mappingMock = new Mock<IMapping>();
        mappingMock.SetupGet(m => m.RequestMatcher).Returns(request);
        mappingMock.SetupGet(m => m.Title).Returns("my title");
        mappingMock.SetupGet(m => m.Description).Returns("my description");

        var requestMessageMock = new Mock<IRequestMessage>();

        var responseMessage = new ResponseMessage();

        // Act
        var proxyMapping = _sut.ToMapping(mappingMock.Object, proxyAndRecordSettings, requestMessageMock.Object, responseMessage);

        // Assert
        var model = _mappingConverter.ToMappingModel(proxyMapping);
        var json = JsonConvert.SerializeObject(model, JsonSerializationConstants.JsonSerializerSettingsDefault);
        var expected = File.ReadAllText(Path.Combine("../../../", "Serialization", "files", "proxy.json"));

        json.Should().Be(expected);
    }
}
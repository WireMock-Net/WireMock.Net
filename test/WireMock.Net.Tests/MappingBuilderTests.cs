using FluentAssertions;
using WireMock.Owin;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests;

public class MappingBuilderTests
{
    private readonly MappingBuilder _sut;

    public MappingBuilderTests()
    {
        var settings = new WireMockServerSettings();
        var options = new WireMockMiddlewareOptions();
        var matcherMapper = new MatcherMapper(settings);
        var mappingConverter = new MappingConverter(matcherMapper);
        var mappingToFileSaver = new MappingToFileSaver(settings, mappingConverter);

        _sut = new MappingBuilder(settings, options, mappingConverter, mappingToFileSaver);

        _sut.Given(Request.Create()
            .WithPath("/foo")
            .UsingGet()
        )
        .RespondWith(Response.Create()
            .WithBody(@"{ msg: ""Hello world!""}")
        );
    }

    [Fact]
    public void GetMappings()
    {
        // Act
        var mappings = _sut.GetMappings();

        // Assert
        mappings.Should().HaveCount(1);
    }
}
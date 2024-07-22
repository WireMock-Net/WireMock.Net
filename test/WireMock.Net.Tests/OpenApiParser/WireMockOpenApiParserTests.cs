#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using VerifyXunit;
using WireMock.Net.OpenApiParser;
using WireMock.Net.OpenApiParser.Settings;
using Xunit;

namespace WireMock.Net.Tests.OpenApiParser;

[UsesVerify]
public class WireMockOpenApiParserTests
{
    private readonly Mock<IWireMockOpenApiParserExampleValues> _exampleValuesMock = new();

    private readonly WireMockOpenApiParser _sut = new();

    public WireMockOpenApiParserTests()
    {
        _exampleValuesMock.SetupGet(e => e.Boolean).Returns(true);
        _exampleValuesMock.SetupGet(e => e.Integer).Returns(42);
        _exampleValuesMock.SetupGet(e => e.Float).Returns(1.1f);
        _exampleValuesMock.SetupGet(e => e.Double).Returns(2.2d);
        _exampleValuesMock.SetupGet(e => e.String).Returns("example-string");
        _exampleValuesMock.SetupGet(e => e.Object).Returns("example-object");
        _exampleValuesMock.SetupGet(e => e.Bytes).Returns("Stef"u8.ToArray());
        _exampleValuesMock.SetupGet(e => e.Date).Returns(() => new DateTime(2024, 6, 19));
        _exampleValuesMock.SetupGet(e => e.DateTime).Returns(() => new DateTime(2024, 6, 19, 12, 34, 56, DateTimeKind.Utc));
    }

    [Fact]
    public async Task FromText_ShouldReturnMappings()
    {
        // Arrange
        var settings = new WireMockOpenApiParserSettings
        {
            ExampleValues = _exampleValuesMock.Object
        };

        var openApiDocument = await File.ReadAllTextAsync(Path.Combine("OpenApiParser", "payroc-openapi-spec.yaml"));

        // Act
        var mappings = _sut.FromText(openApiDocument, settings, out _);

        // Verify
        await Verifier.Verify(mappings);
    }
}
#endif
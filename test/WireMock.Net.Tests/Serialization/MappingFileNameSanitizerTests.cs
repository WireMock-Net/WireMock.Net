using System;
using Moq;
using WireMock.Serialization;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public class MappingFileNameSanitizerTests
{
    private readonly string _mappingGuid = "ce216a13-e7d6-42d7-91ac-8ae709e2add1";
    private readonly string _mappingTitle =
        "Proxy Mapping for POST _ordermanagement_v1_orders_cancel";

    [Fact]
    public void BuildSanitizedFileName_WithTitleAndGuid_AppendsGuid()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(_mappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(_mappingGuid));

        var settingsMock = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                AppendGuidToSavedMappingFile = true
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settingsMock);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"POST_ordermanagement_v1_orders_cancel_{_mappingGuid}.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithoutTitle_UsesGuid()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns((string?)null);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(_mappingGuid));

        var settingsMock = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ()
        };
        var sanitizer = new MappingFileNameSanitizer(settingsMock);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"{_mappingGuid}.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithTitleAndGuid_NoAppendGuidSetting()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(_mappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(_mappingGuid));

        var settingsMock = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                AppendGuidToSavedMappingFile = false
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settingsMock);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal("POST_ordermanagement_v1_orders_cancel.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithPrefix_AddsPrefix()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(_mappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(_mappingGuid));

        var settingsMock = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                PrefixForSavedMappingFile = "Prefix"
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settingsMock);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"Prefix_POST_ordermanagement_v1_orders_cancel.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithTitleAndGuid_WithPrefixAndAppendGuidSetting()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(_mappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(_mappingGuid));

        var settingsMock = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                PrefixForSavedMappingFile = "Prefix",
                AppendGuidToSavedMappingFile = true
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settingsMock);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"Prefix_POST_ordermanagement_v1_orders_cancel_{_mappingGuid}.json", result);
    }
}

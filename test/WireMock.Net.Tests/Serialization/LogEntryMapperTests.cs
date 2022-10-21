using FluentAssertions;
using NFluent;
using WireMock.Logging;
using WireMock.Models;
using WireMock.Owin;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Serialization;

public class LogEntryMapperTests
{
    private readonly IWireMockMiddlewareOptions _options = new WireMockMiddlewareOptions();

    private readonly LogEntryMapper _sut;

    public LogEntryMapperTests()
    {
        _sut = new LogEntryMapper(_options);
    }

    [Fact]
    public void LogEntryMapper_Map_LogEntry_Check_BodyTypeBytes()
    {
        // Assign
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(
                new UrlDetails("http://localhost"),
                "post",
                "::1",
                new BodyData
                {
                    DetectedBodyType = BodyType.Bytes,
                    BodyAsBytes = new byte[] { 0 }
                }
            ),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.Bytes,
                    BodyAsBytes = new byte[] { 0 }
                }
            }
        };

        // Act
        var result = _sut.Map(logEntry);

        // Assert
        Check.That(result.Request.DetectedBodyType).IsEqualTo("Bytes");
        Check.That(result.Request.DetectedBodyTypeFromContentType).IsNull();
        Check.That(result.Request.BodyAsBytes).ContainsExactly(new byte[] { 0 });
        Check.That(result.Request.Body).IsNull();
        Check.That(result.Request.BodyAsJson).IsNull();

        Check.That(result.Response.DetectedBodyType).IsEqualTo(BodyType.Bytes);
        Check.That(result.Response.DetectedBodyTypeFromContentType).IsNull();
        Check.That(result.Response.BodyAsBytes).ContainsExactly(new byte[] { 0 });
        Check.That(result.Response.Body).IsNull();
        Check.That(result.Response.BodyAsJson).IsNull();
        Check.That(result.Response.BodyAsFile).IsNull();
    }

    [Fact]
    public void LogEntryMapper_Map_LogEntry_Check_ResponseBodyTypeFile()
    {
        // Assign
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(new UrlDetails("http://localhost"), "get", "::1"),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.File,
                    BodyAsFile = "test"
                }
            }
        };

        // Act
        var result = _sut.Map(logEntry);

        // Assert
        Check.That(result.Request.DetectedBodyType).IsNull();
        Check.That(result.Request.DetectedBodyTypeFromContentType).IsNull();
        Check.That(result.Request.BodyAsBytes).IsNull();
        Check.That(result.Request.Body).IsNull();
        Check.That(result.Request.BodyAsJson).IsNull();

        Check.That(result.Response.DetectedBodyType).IsEqualTo(BodyType.File);
        Check.That(result.Response.DetectedBodyTypeFromContentType).IsNull();
        Check.That(result.Request.BodyAsBytes).IsNull();
        Check.That(result.Response.Body).IsNull();
        Check.That(result.Response.BodyAsJson).IsNull();
        Check.That(result.Response.BodyAsFile).IsEqualTo("test");
    }

    [Fact]
    public void LogEntryMapper_Map_LogEntry_WithFault()
    {
        // Assign
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(new UrlDetails("http://localhost"), "get", "::1"),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.File,
                    BodyAsFile = "test"
                },
                FaultType = FaultType.EMPTY_RESPONSE,
                FaultPercentage = 0.5
            }
        };

        // Act
        var result = _sut.Map(logEntry);

        // Assert
        result.Response.FaultType.Should().Be("EMPTY_RESPONSE");
        result.Response.FaultPercentage.Should().Be(0.5);
    }

    [Fact]
    public void LogEntryMapper_Map_LogEntry_WhenFuncIsUsed_And_DoNotSaveDynamicResponseInLogEntry_Is_True_Should_NotSave_StringResponse()
    {
        // Assign
        var options = new WireMockMiddlewareOptions
        {
            DoNotSaveDynamicResponseInLogEntry = true
        };
        var isFuncUsed = "Func<IRequestMessage, string>";
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(
                new UrlDetails("http://localhost"),
                "post",
                "::1",
                new BodyData
                {
                    DetectedBodyType = BodyType.Bytes,
                    BodyAsBytes = new byte[] { 0 }
                }
            ),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = "test",
                    IsFuncUsed = isFuncUsed
                }
            }
        };

        // Act
        var result = new LogEntryMapper(options).Map(logEntry);

        // Assert
        result.Response.Body.Should().Be(isFuncUsed);
    }
}
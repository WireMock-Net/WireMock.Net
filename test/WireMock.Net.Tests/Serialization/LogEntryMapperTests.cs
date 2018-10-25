using NFluent;
using WireMock.Logging;
using WireMock.Models;
using WireMock.Serialization;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Serialization
{
    public class LogEntryMapperTests
    {
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
            var result = LogEntryMapper.Map(logEntry);

            // Assert
            Check.That(result.Request.DetectedBodyType).IsEqualTo("Bytes");
            Check.That(result.Request.DetectedBodyTypeFromContentType).IsEqualTo("None");
            Check.That(result.Request.BodyAsBytes).ContainsExactly(new byte[] { 0 });
            Check.That(result.Request.Body).IsNull();
            Check.That(result.Request.BodyAsJson).IsNull();

            Check.That(result.Response.DetectedBodyType).IsEqualTo(BodyType.Bytes);
            Check.That(result.Response.DetectedBodyTypeFromContentType).IsEqualTo(BodyType.None);
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
                RequestMessage = new RequestMessage(new UrlDetails("http://localhost"), "get", "::1"
                ),
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
            var result = LogEntryMapper.Map(logEntry);

            // Assert
            Check.That(result.Request.DetectedBodyType).IsNull();
            Check.That(result.Request.DetectedBodyTypeFromContentType).IsNull();
            Check.That(result.Request.BodyAsBytes).IsNull();
            Check.That(result.Request.Body).IsNull();
            Check.That(result.Request.BodyAsJson).IsNull();

            Check.That(result.Response.DetectedBodyType).IsEqualTo(BodyType.File);
            Check.That(result.Response.DetectedBodyTypeFromContentType).IsEqualTo(BodyType.None);
            Check.That(result.Request.BodyAsBytes).IsNull();
            Check.That(result.Response.Body).IsNull();
            Check.That(result.Response.BodyAsJson).IsNull();
            Check.That(result.Response.BodyAsFile).IsEqualTo("test");
        }
    }
}
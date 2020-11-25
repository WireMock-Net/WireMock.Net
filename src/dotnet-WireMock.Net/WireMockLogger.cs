using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace WireMock
{
    public class WireMockLogger : IWireMockLogger
    {
        private readonly ILogger _logger;

        public WireMockLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <see cref="IWireMockLogger.Debug"/>
        public void Debug(string formatString, params object[] args)
        {
            _logger.LogDebug(formatString, args);
        }

        /// <see cref="IWireMockLogger.Info"/>
        public void Info(string formatString, params object[] args)
        {
            _logger.LogInformation(formatString, args);
        }

        /// <see cref="IWireMockLogger.Warn"/>
        public void Warn(string formatString, params object[] args)
        {
            _logger.LogWarning(formatString, args);
        }

        /// <see cref="IWireMockLogger.Error(string, object[])"/>
        public void Error(string formatString, params object[] args)
        {
            _logger.LogError(formatString, args);
        }

        /// <see cref="IWireMockLogger.Error(string, Exception)"/>
        public void Error(string formatString, Exception exception)
        {
            _logger.LogError(formatString, exception);
        }

        /// <see cref="IWireMockLogger.DebugRequestResponse"/>
        public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
        {
            string message = JsonSerializer.Serialize(logEntryModel);

            _logger.LogDebug("Admin[{IsAdmin}] {Message}", isAdminRequest, message);
        }
    }
}
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace WireMock.Net.WebApp
{
    public class WireMockLogger : IWireMockLogger
    {
        private readonly ILogger<WireMockService> _logger;

        public WireMockLogger(ILogger<WireMockService> logger)
        {
            _logger = logger;
        }

        public void Debug(string formatString, params object[] args)
        {
            _logger.LogDebug(formatString, args);
        }

        public void Info(string formatString, params object[] args)
        {
            _logger.LogInformation(formatString, args);
        }

        public void Warn(string formatString, params object[] args)
        {
            _logger.LogWarning(formatString, args);
        }

        public void Error(string formatString, params object[] args)
        {
            _logger.LogError(formatString, args);
        }

        public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
        {
            string message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
            _logger.LogDebug("Admin[{0}] {1}", isAdminRequest, message);
        }
    }
}

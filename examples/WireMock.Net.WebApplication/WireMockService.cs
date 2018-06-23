using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Settings;

namespace WireMock.Net.WebApplication
{
    public class WireMockService : IWireMockService
    {
        private static int sleepTime = 30000;

        private readonly ILogger _logger;
        private readonly IFluentMockServerSettings _settings;

        private class Logger : IWireMockLogger
        {
            private readonly ILogger _logger;

            public Logger(ILogger logger)
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

            public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminrequest)
            {
                string message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
                _logger.LogDebug("Admin[{0}] {1}", isAdminrequest, message);
            }
        }

        public WireMockService(ILogger logger, IFluentMockServerSettings settings)
        {
            _logger = logger;
            _settings = settings;

            _settings.Logger = new Logger(logger);
        }

        public void Run()
        {
            _logger.LogInformation("WireMock.Net server starting");

            StandAloneApp.Start(_settings);

            _logger.LogInformation($"WireMock.Net server settings {JsonConvert.SerializeObject(_settings)}");

            while (true)
            {
                _logger.LogInformation("WireMock.Net server running");
                Thread.Sleep(sleepTime);
            }
        }
    }
}
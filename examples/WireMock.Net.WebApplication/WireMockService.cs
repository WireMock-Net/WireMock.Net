using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WireMock.Net.StandAlone;
using WireMock.Settings;

namespace WireMock.Net.WebApplication
{
    public class WireMockService : IWireMockService
    {
        private static int sleepTime = 30000;

        private readonly ILogger _logger;
        private readonly IFluentMockServerSettings _settings;

        public WireMockService(ILogger logger, IFluentMockServerSettings settings)
        {
            _logger = logger;
            _settings = settings;
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
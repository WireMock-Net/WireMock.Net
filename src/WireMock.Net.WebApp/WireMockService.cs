using Newtonsoft.Json;
using System.Threading;
using WireMock.Logging;
using WireMock.Net.StandAlone;
using WireMock.Settings;

namespace WireMock.Net.WebApp
{
    public class WireMockService : IWireMockService
    {
        private static int sleepTime = 30000;

        private readonly IWireMockLogger _logger;
        private readonly IFluentMockServerSettings _settings;

        public WireMockService(IWireMockLogger logger, IFluentMockServerSettings settings)
        {
            _logger = logger;
            _settings = settings;

            _settings.Logger = logger;
        }

        public void Run()
        {
            _logger.Info("WireMock.Net server starting");

            StandAloneApp.Start(_settings);

            _logger.Info($"WireMock.Net server settings {JsonConvert.SerializeObject(_settings)}");

            while (true)
            {
                _logger.Info("WireMock.Net server running");
                Thread.Sleep(sleepTime);
            }
        }
    }
}
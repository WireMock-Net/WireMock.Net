using Microsoft.Extensions.Logging;

namespace WireMock.Net.WebApplication
{
    public class App
    {
        private readonly IWireMockService _service;
        private readonly ILogger _logger;
        
        public App(IWireMockService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("WireMock.Net App running");
            _service.Run();
        }
    }
}
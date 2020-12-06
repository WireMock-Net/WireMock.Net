using System;
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders
{
    internal class ProxyAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, IWireMockServerSettings, Task<ResponseMessage>> _responseMessageFunc;
        private readonly IWireMockServerSettings _settings;

        public ProxyAsyncResponseProvider(Func<RequestMessage, IWireMockServerSettings, Task<ResponseMessage>> responseMessageFunc, IWireMockServerSettings settings)
        {
            _responseMessageFunc = responseMessageFunc;
            _settings = settings;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage, IWireMockServerSettings settings, Action<IMapping> action)
        {
            return _responseMessageFunc(requestMessage, _settings);
        }
    }
}
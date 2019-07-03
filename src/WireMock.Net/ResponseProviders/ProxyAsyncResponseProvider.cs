using System;
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders
{
    internal class ProxyAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, IFluentMockServerSettings, Task<ResponseMessage>> _responseMessageFunc;
        private readonly IFluentMockServerSettings _settings;

        public ProxyAsyncResponseProvider(Func<RequestMessage, IFluentMockServerSettings, Task<ResponseMessage>> responseMessageFunc, IFluentMockServerSettings settings)
        {
            _responseMessageFunc = responseMessageFunc;
            _settings = settings;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage, IFluentMockServerSettings settings)
        {
            return _responseMessageFunc(requestMessage, _settings);
        }
    }
}
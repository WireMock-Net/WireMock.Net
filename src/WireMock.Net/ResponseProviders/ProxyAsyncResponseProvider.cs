using System;
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders
{
    internal class ProxyAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, WireMockServerSettings, Task<ResponseMessage>> _responseMessageFunc;
        private readonly WireMockServerSettings _settings;

        public ProxyAsyncResponseProvider(Func<RequestMessage, WireMockServerSettings, Task<ResponseMessage>> responseMessageFunc, WireMockServerSettings settings)
        {
            _responseMessageFunc = responseMessageFunc;
            _settings = settings;
        }

        public async Task<(ResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(RequestMessage requestMessage, WireMockServerSettings settings)
        {
            return (await _responseMessageFunc(requestMessage, _settings).ConfigureAwait(false), null);
        }
    }
}
using System;
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders
{
    internal class ProxyAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<IRequestMessage, WireMockServerSettings, Task<IResponseMessage>> _responseMessageFunc;
        private readonly WireMockServerSettings _settings;

        public ProxyAsyncResponseProvider(Func<IRequestMessage, WireMockServerSettings, Task<IResponseMessage>> responseMessageFunc, WireMockServerSettings settings)
        {
            _responseMessageFunc = responseMessageFunc;
            _settings = settings;
        }

        public async Task<(IResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(IRequestMessage requestMessage, WireMockServerSettings settings)
        {
            return (await _responseMessageFunc(requestMessage, _settings).ConfigureAwait(false), null);
        }
    }
}
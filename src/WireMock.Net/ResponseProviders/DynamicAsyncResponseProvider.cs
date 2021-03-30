using System;
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders
{
    internal class DynamicAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, Task<ResponseMessage>> _responseMessageFunc;

        public DynamicAsyncResponseProvider(Func<RequestMessage, Task<ResponseMessage>> responseMessageFunc)
        {
            _responseMessageFunc = responseMessageFunc;
        }

        public async Task<(ResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(RequestMessage requestMessage, IWireMockServerSettings settings)
        {
            return (await _responseMessageFunc(requestMessage), null);
        }
    }
}
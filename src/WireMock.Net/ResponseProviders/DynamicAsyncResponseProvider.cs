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

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage, IWireMockServerSettings settings)
        {
            return _responseMessageFunc(requestMessage);
        }
    }
}
using System;
using System.Threading.Tasks;
using WireMock.Handlers;
using WireMock.Settings;

namespace WireMock.ResponseProviders
{
    internal class DynamicResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, ResponseMessage> _responseMessageFunc;

        public DynamicResponseProvider(Func<RequestMessage, ResponseMessage> responseMessageFunc)
        {
            _responseMessageFunc = responseMessageFunc;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage, IFluentMockServerSettings settings)
        {
            return Task.FromResult(_responseMessageFunc(requestMessage));
        }
    }
}
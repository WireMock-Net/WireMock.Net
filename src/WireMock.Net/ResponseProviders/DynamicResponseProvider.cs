using System;
using System.Threading.Tasks;
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

        public Task<(ResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(RequestMessage requestMessage, IWireMockServerSettings settings)
        {
            (ResponseMessage responseMessage, IMapping mapping) result = (_responseMessageFunc(requestMessage), null);
            return Task.FromResult(result);
        }
    }
}
using System;
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders
{
    internal class DynamicResponseProvider : IResponseProvider
    {
        private readonly Func<IRequestMessage, IResponseMessage> _responseMessageFunc;

        public DynamicResponseProvider(Func<IRequestMessage, IResponseMessage> responseMessageFunc)
        {
            _responseMessageFunc = responseMessageFunc;
        }

        public Task<(IResponseMessage Message, IMapping Mapping)> ProvideResponseAsync(IRequestMessage requestMessage, WireMockServerSettings settings)
        {
            (IResponseMessage responseMessage, IMapping mapping) result = (_responseMessageFunc(requestMessage), null);
            return Task.FromResult(result);
        }
    }
}
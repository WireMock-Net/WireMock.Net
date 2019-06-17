using System;
using System.Threading.Tasks;
using WireMock.Handlers;

namespace WireMock.ResponseProviders
{
    internal class DynamicResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, ResponseMessage> _responseMessageFunc;

        public DynamicResponseProvider(Func<RequestMessage, ResponseMessage> responseMessageFunc)
        {
            _responseMessageFunc = responseMessageFunc;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage, IFileSystemHandler fileSystemHandler)
        {
            return Task.FromResult(_responseMessageFunc(requestMessage));
        }
    }
}
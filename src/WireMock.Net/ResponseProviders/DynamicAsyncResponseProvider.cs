using System;
using System.Threading.Tasks;
using WireMock.Handlers;

namespace WireMock.ResponseProviders
{
    internal class DynamicAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, Task<ResponseMessage>> _responseMessageFunc;

        public DynamicAsyncResponseProvider(Func<RequestMessage, Task<ResponseMessage>> responseMessageFunc)
        {
            _responseMessageFunc = responseMessageFunc;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage, IFileSystemHandler fileSystemHandler)
        {
            return _responseMessageFunc(requestMessage);
        }
    }
}
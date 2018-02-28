using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.ResponseProviders
{
    internal class DynamicAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, Task<ResponseMessage>> _responseMessageFunc;

        public DynamicAsyncResponseProvider([NotNull] Func<RequestMessage, Task<ResponseMessage>> responseMessageFunc)
        {
            Check.NotNull(responseMessageFunc, nameof(responseMessageFunc));

            _responseMessageFunc = responseMessageFunc;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage)
        {
            return _responseMessageFunc(requestMessage);
        }
    }
}
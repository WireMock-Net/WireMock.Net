using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock
{
    internal class DynamicResponseProvider : IResponseProvider
    {
        private readonly Func<ResponseMessage> _responseMessageFunc;

        public DynamicResponseProvider([NotNull] Func<ResponseMessage> responseMessageFunc)
        {
            Check.NotNull(responseMessageFunc, nameof(responseMessageFunc));

            _responseMessageFunc = responseMessageFunc;
        }

        public Task<ResponseMessage> ProvideResponse(RequestMessage requestMessage)
        {
            return Task.FromResult(_responseMessageFunc());
        }
    }
}
using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Validation;
using WireMock.Settings;

namespace WireMock
{
    internal class DynamicResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, ResponseMessage> _responseMessageFunc;

        public DynamicResponseProvider([NotNull] Func<RequestMessage, ResponseMessage> responseMessageFunc)
        {
            Check.NotNull(responseMessageFunc, nameof(responseMessageFunc));

            _responseMessageFunc = responseMessageFunc;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage)
        {
            return Task.FromResult(_responseMessageFunc(requestMessage));
        }
    }

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

    internal class ProxyAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, ProxyAndRecordSettings, Task<ResponseMessage>> _responseMessageFunc;
        private readonly ProxyAndRecordSettings _settings;

        public ProxyAsyncResponseProvider([NotNull] Func<RequestMessage, ProxyAndRecordSettings, Task<ResponseMessage>> responseMessageFunc, [NotNull] ProxyAndRecordSettings settings)
        {
            Check.NotNull(responseMessageFunc, nameof(responseMessageFunc));
            Check.NotNull(settings, nameof(settings));

            _responseMessageFunc = responseMessageFunc;
            _settings = settings;
        }

        public Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage)
        {
            return _responseMessageFunc(requestMessage, _settings);
        }
    }
}
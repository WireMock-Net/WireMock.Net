using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Settings;
using WireMock.Validation;

namespace WireMock.ResponseProviders
{
    internal class ProxyAsyncResponseProvider : IResponseProvider
    {
        private readonly Func<RequestMessage, IProxyAndRecordSettings, Task<ResponseMessage>> _responseMessageFunc;
        private readonly IProxyAndRecordSettings _settings;

        public ProxyAsyncResponseProvider([NotNull] Func<RequestMessage, IProxyAndRecordSettings, Task<ResponseMessage>> responseMessageFunc, [NotNull] IProxyAndRecordSettings settings)
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
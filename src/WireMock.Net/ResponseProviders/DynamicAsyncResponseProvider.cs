using System;
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

internal class DynamicAsyncResponseProvider : IResponseProvider
{
    private readonly Func<IRequestMessage, Task<IResponseMessage>> _responseMessageFunc;

    public DynamicAsyncResponseProvider(Func<IRequestMessage, Task<IResponseMessage>> responseMessageFunc)
    {
        _responseMessageFunc = responseMessageFunc;
    }

    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        return (await _responseMessageFunc(requestMessage).ConfigureAwait(false), null);
    }
}
// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

internal class ProxyAsyncResponseProvider : IResponseProvider
{
    private readonly Func<IRequestMessage, WireMockServerSettings, Task<IResponseMessage>> _responseMessageFunc;
    private readonly WireMockServerSettings _settings;

    public ProxyAsyncResponseProvider(Func<IRequestMessage, WireMockServerSettings, Task<IResponseMessage>> responseMessageFunc, WireMockServerSettings settings)
    {
        _responseMessageFunc = Guard.NotNull(responseMessageFunc);
        _settings = Guard.NotNull(settings);
    }

    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        return (await _responseMessageFunc(requestMessage, _settings).ConfigureAwait(false), null);
    }
}
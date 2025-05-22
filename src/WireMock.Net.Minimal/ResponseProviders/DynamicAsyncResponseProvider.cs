// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

internal class DynamicAsyncResponseProvider : IResponseProvider
{
    private readonly Func<IRequestMessage, Task<IResponseMessage>> _responseMessageFunc;

    public DynamicAsyncResponseProvider(Func<IRequestMessage, Task<IResponseMessage>> responseMessageFunc)
    {
        _responseMessageFunc = Guard.NotNull(responseMessageFunc);
    }

    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        return (await _responseMessageFunc(requestMessage).ConfigureAwait(false), null);
    }
}
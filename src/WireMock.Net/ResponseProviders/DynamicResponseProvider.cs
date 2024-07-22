// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

internal class DynamicResponseProvider : IResponseProvider
{
    private readonly Func<IRequestMessage, IResponseMessage> _responseMessageFunc;

    public DynamicResponseProvider(Func<IRequestMessage, IResponseMessage> responseMessageFunc)
    {
        _responseMessageFunc = Guard.NotNull(responseMessageFunc);
    }

    public Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        (IResponseMessage responseMessage, IMapping? mapping) result = (_responseMessageFunc(requestMessage), null);
        return Task.FromResult(result);
    }
}
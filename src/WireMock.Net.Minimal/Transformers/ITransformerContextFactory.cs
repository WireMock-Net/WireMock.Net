// Copyright © WireMock.Net

namespace WireMock.Transformers
{
    interface ITransformerContextFactory
    {
        ITransformerContext Create();
    }
}
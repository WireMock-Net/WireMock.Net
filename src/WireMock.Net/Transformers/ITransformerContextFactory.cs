namespace WireMock.Transformers
{
    interface ITransformerContextFactory<T> where T : ITransformerContext
    {
        T Create();
    }
}
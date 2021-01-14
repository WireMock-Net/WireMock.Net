namespace WireMock.Transformers
{
    interface ITransformerContextFactory<out T> where T : ITransformerContext
    {
        T Create();
    }
}
namespace WireMock.Transformers
{
    interface ITransformer
    {
        ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile);
    }
}
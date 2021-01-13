namespace WireMock.Transformers
{
    interface IResponseMessageTransformer
    {
        ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile);
    }
}
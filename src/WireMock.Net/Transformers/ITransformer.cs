using System;
using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers
{
    interface ITransformer
    {
        ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile, ReplaceNodeOptions options);

        (IBodyData BodyData, IDictionary<string, WireMockList<string>> Headers) Transform(RequestMessage originalRequestMessage, ResponseMessage originalResponseMessage, IBodyData bodyData, IDictionary<string, WireMockList<string>> headers, ReplaceNodeOptions options);
    }
}
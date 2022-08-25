using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

interface ITransformer
{
    ResponseMessage Transform(IMapping mapping, IRequestMessage requestMessage, IResponseMessage original, bool useTransformerForBodyAsFile, ReplaceNodeOptions options);

    (IBodyData? BodyData, IDictionary<string, WireMockList<string>>? Headers) Transform(IMapping mapping, IRequestMessage originalRequestMessage, IResponseMessage originalResponseMessage, IBodyData? bodyData, IDictionary<string, WireMockList<string>>? headers, ReplaceNodeOptions options);
}
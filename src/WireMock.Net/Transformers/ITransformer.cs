// Copyright © WireMock.Net

using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

interface ITransformer
{
    ResponseMessage Transform(IMapping mapping, IRequestMessage requestMessage, IResponseMessage original, bool useTransformerForBodyAsFile, ReplaceNodeOptions options);

    IBodyData? TransformBody(IMapping mapping, IRequestMessage originalRequestMessage, IResponseMessage originalResponseMessage, IBodyData? bodyData, ReplaceNodeOptions options);

    IDictionary<string, WireMockList<string>> TransformHeaders(IMapping mapping, IRequestMessage originalRequestMessage, IResponseMessage originalResponseMessage, IDictionary<string, WireMockList<string>>? headers);

    string TransformString(IMapping mapping, IRequestMessage originalRequestMessage, IResponseMessage originalResponseMessage, string? value);
}
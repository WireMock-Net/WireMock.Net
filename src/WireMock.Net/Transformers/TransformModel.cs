using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WireMock.Transformers;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal struct TransformModel
{
    public IMapping mapping { get; set; }

    public IRequestMessage request { get; set; }

    public IResponseMessage? response { get; set; }

    public IDictionary<string, object?> data { get; set; }
}
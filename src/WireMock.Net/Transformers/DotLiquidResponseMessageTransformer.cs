using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireMock.Transformers
{
    internal class DotLiquidResponseMessageTransformer : IResponseMessageTransformer
    {
        public ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original, bool useTransformerForBodyAsFile)
        {
            return null;
        }
    }
}

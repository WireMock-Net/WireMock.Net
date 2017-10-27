using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;
using WireMock.Util;

namespace WireMock.Transformers
{
    internal static class ResponseMessageTransformer
    {
        public static ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original)
        {
            var responseMessage = new ResponseMessage { StatusCode = original.StatusCode, BodyOriginal = original.Body };

            var template = new { request = requestMessage };

            // Body
            var templateBody = Handlebars.Compile(original.Body);
            responseMessage.Body = templateBody(template);

            // Headers
            var newHeaders = new Dictionary<string, WireMockList<string>>();
            foreach (var header in original.Headers)
            {
                var templateHeaderKey = Handlebars.Compile(header.Key);
                var templateHeaderValues = header.Value
                    .Select(Handlebars.Compile)
                    .Select(func => func(template))
                    .ToArray();

                newHeaders.Add(templateHeaderKey(template), new WireMockList<string>(templateHeaderValues));
            }

            responseMessage.Headers = newHeaders;

            return responseMessage;
        }
    }
}
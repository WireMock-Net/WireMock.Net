using HandlebarsDotNet;
using System.Collections.Generic;

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
            var newHeaders = new Dictionary<string, string>();
            foreach (var header in original.Headers)
            {
                var templateHeaderKey = Handlebars.Compile(header.Key);
                var templateHeaderValue = Handlebars.Compile(header.Value);

                newHeaders.Add(templateHeaderKey(template), templateHeaderValue(template));
            }

            responseMessage.Headers = newHeaders;

            return responseMessage;
        }
    }
}
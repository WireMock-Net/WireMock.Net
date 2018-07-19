using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet;
using Newtonsoft.Json;
using WireMock.Util;

namespace WireMock.Transformers
{
    internal static class ResponseMessageTransformer
    {
        static ResponseMessageTransformer()
        {
            HandlebarsHelpers.Register();
        }

        public static ResponseMessage Transform(RequestMessage requestMessage, ResponseMessage original)
        {
            bool bodyIsJson = original.BodyAsJson != null;
            var responseMessage = new ResponseMessage { StatusCode = original.StatusCode };

            if (!bodyIsJson)
            {
                responseMessage.BodyOriginal = original.Body;
            }

            var template = new { request = requestMessage };

            // Body
            Formatting formatting = original.BodyAsJsonIndented == true ? Formatting.Indented : Formatting.None;
            string body = bodyIsJson ? JsonConvert.SerializeObject(original.BodyAsJson, formatting) : original.Body;
            if (body != null)
            {
                var templateBody = Handlebars.Compile(body);

                if (!bodyIsJson)
                {
                    responseMessage.Body = templateBody(template);
                }
                else
                {
                    responseMessage.BodyAsJson = JsonConvert.DeserializeObject(templateBody(template));
                }
            }

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
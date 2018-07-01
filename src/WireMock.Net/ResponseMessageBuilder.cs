using System;
using System.Collections.Generic;
using WireMock.Admin.Mappings;
using WireMock.Http;
using WireMock.Util;

namespace WireMock
{
    internal static class ResponseMessageBuilder
    {
        private static string ContentTypeJson = "application/json";
        private static readonly IDictionary<string, WireMockList<string>> ContentTypeJsonHeaders = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string> { ContentTypeJson } } };

        internal static ResponseMessage Create(string message, int statusCode = 200, Guid? guid = null)
        {
            var response = new ResponseMessage
            {
                StatusCode = statusCode,
                Headers = ContentTypeJsonHeaders,
                BodyAsJson = message != null ? new StatusModel { Status = message, Guid = guid } : null
            };

            return response;
        }
    }
}
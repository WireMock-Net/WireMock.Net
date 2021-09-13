using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Util;
using WireMock.Validation;
using OrgMapping = WireMock.Org.Abstractions.Mapping;

namespace WireMock.Server
{
    public partial class WireMockServer
    {
        [PublicAPI]
        public void ReadStaticWireMockOrgMapping([NotNull] string path)
        {
            Check.NotNull(path, nameof(path));

            if (FileHelper.TryReadMappingFileWithRetryAndDelay(_settings.FileSystemHandler, path, out string value))
            {
                var mappings = DeserializeJsonToArray<OrgMapping>(value);
                foreach (var mapping in mappings)
                {
                    Map(mapping);
                }
            }
        }

        private ResponseMessage MappingsPostWireMockOrg(RequestMessage requestMessage)
        {
            try
            {
                var mappingModels = DeserializeRequestMessageToArray<OrgMapping>(requestMessage);
                if (mappingModels.Length == 1)
                {
                    Guid? guid = Map(mappingModels[0]);
                    return ResponseMessageBuilder.Create("Mapping added", 201, guid);
                }

                foreach (var mappingModel in mappingModels)
                {
                    Map(mappingModel);
                }

                return ResponseMessageBuilder.Create("Mappings added", 201);
            }
            catch (ArgumentException a)
            {
                _settings.Logger.Error("HttpStatusCode set to 400 {0}", a);
                return ResponseMessageBuilder.Create(a.Message, 400);
            }
            catch (Exception e)
            {
                _settings.Logger.Error("HttpStatusCode set to 500 {0}", e);
                return ResponseMessageBuilder.Create(e.ToString(), 500);
            }
        }

        private Guid? Map(OrgMapping mapping)
        {
            var requestBuilder = Request.Create();

            var request = mapping.Request;
            if (request != null)
            {
                if (request.Url != null)
                {
                    requestBuilder = requestBuilder.WithUrl(request.Url);
                }
                else if (request.UrlPattern != null)
                {
                    requestBuilder = requestBuilder.WithUrl(new RegexMatcher(request.UrlPattern));
                }
                else if (request.UrlPath != null)
                {
                    requestBuilder = requestBuilder.WithPath(request.Url);
                }
                else if (request.UrlPathPattern != null)
                {
                    requestBuilder = requestBuilder.WithPath(new RegexMatcher(request.UrlPathPattern));
                }

                if (request.Method != null)
                {
                    requestBuilder = requestBuilder.UsingMethod(request.Method);
                }

                /*
                "headers" : {
                  "Accept" : {
                    "contains" : "xml"
                  }
                }
                */
                if (request.Headers is JObject headers)
                {
                    ProcessWireMockOrgJObjectAndUseStringMatcher(headers, (key, match) =>
                    {
                        requestBuilder = requestBuilder.WithHeader(key, match as IStringMatcher);
                    });
                }

                if (request.Cookies is JObject cookies)
                {
                    ProcessWireMockOrgJObjectAndUseStringMatcher(cookies, (key, match) =>
                    {
                        requestBuilder = requestBuilder.WithCookie(key, match as IStringMatcher);
                    });
                }

                /*
                "queryParameters" : {
                  "search_term" : {
                    "equalTo" : "WireMock"
                  }
                }
                */
                if (request.QueryParameters is JObject queryParameters)
                {
                    ProcessWireMockOrgJObjectAndUseStringMatcher(queryParameters, (key, match) =>
                    {
                        requestBuilder = requestBuilder.WithParam(key, match);
                    });
                }

                /*
                 "bodyPatterns" : [ {
                  "equalToJson" : "{ "cityName": "São Paulo", "cityCode": 5001 },
                  "ignoreArrayOrder" : true,
                  "ignoreExtraElements" : true
                } ]
                */
                if (request.BodyPatterns?.Any() == true)
                {
                    var jObjectArray = request.BodyPatterns.Cast<JObject>();
                    var bodyPattern = jObjectArray.First();
                    ProcessWireMockOrgJObjectAndUseIMatcher(bodyPattern, (match) =>
                    {
                        requestBuilder = requestBuilder.WithBody(match);
                    });
                }
            }

            IResponseBuilder responseBuilder = Response.Create();

            var response = mapping.Response;
            if (response != null)
            {
                responseBuilder = responseBuilder.WithStatusCode(response.Status);

                if (response.Headers is JObject responseHeaders)
                {
                    ProcessWireMockOrgJObjectAndConvertToIDictionary(responseHeaders, (headers) =>
                    {
                        responseBuilder = responseBuilder.WithHeaders(headers);
                    });
                }

                if (response.Transformers != null)
                {
                    responseBuilder = responseBuilder.WithTransformer();
                }

                if (response.Body != null)
                {
                    responseBuilder = responseBuilder.WithBody(response.Body);
                }

                if (response.JsonBody != null)
                {
                    responseBuilder = responseBuilder.WithBodyAsJson(response.JsonBody);
                }

                if (response.Base64Body != null)
                {
                    responseBuilder = responseBuilder.WithBody(Encoding.UTF8.GetString(Convert.FromBase64String(response.Base64Body)));
                }

                if (response.BodyFileName != null)
                {
                    responseBuilder = responseBuilder.WithBodyFromFile(response.BodyFileName);
                }
            }

            var respondProvider = Given(requestBuilder);
            if (mapping.Uuid != null)
            {
                respondProvider = respondProvider.WithGuid(new Guid(mapping.Uuid));
            }

            if (mapping.Name != null)
            {
                respondProvider = respondProvider.WithTitle(mapping.Name);
            }

            respondProvider.RespondWith(responseBuilder);

            return respondProvider.Guid;
        }

        private void ProcessWireMockOrgJObjectAndConvertToIDictionary(JObject items, Action<IDictionary<string, string>> action)
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in items)
            {
                var key = item.Key;
                var valueAsString = item.Value.Value<string>();
                dict.Add(key, valueAsString);
            }

            action(dict);
        }

        private void ProcessWireMockOrgJObjectAndUseStringMatcher(JObject items, Action<string, IStringMatcher> action)
        {
            foreach (var item in items)
            {
                var key = item.Key;
                var match = item.Value.First as JProperty;
                var valueAsString = match?.Value.Value<string>();
                if (string.IsNullOrEmpty(valueAsString))
                {
                    continue;
                }

                IStringMatcher matcher = null;
                switch (match?.Name)
                {
                    case "contains":
                    case "matches":
                        matcher = new WildcardMatcher(valueAsString);
                        break;

                    case "equalTo":
                        matcher = new ExactMatcher(valueAsString);
                        break;
                }

                if (matcher != null)
                {
                    action(key, matcher);
                }
            }
        }

        private void ProcessWireMockOrgJObjectAndUseIMatcher(JObject items, Action<IMatcher> action)
        {
            IMatcher matcher = null;
            foreach (var item in items)
            {
                switch (item.Key)
                {
                    case "equalToJson":
                        matcher = new JsonMatcher(item.Value);
                        break;
                }
            }

            if (matcher != null)
            {
                action(matcher);
            }
        }
    }
}
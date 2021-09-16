using System;
using System.Collections.Generic;
using System.IO;
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
        public void ReadStaticWireMockOrgMappingAndAddOrUpdate([NotNull] string path)
        {
            Check.NotNull(path, nameof(path));

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            if (FileHelper.TryReadMappingFileWithRetryAndDelay(_settings.FileSystemHandler, path, out string value))
            {
                var mappings = DeserializeJsonToArray<OrgMapping>(value);
                foreach (var mapping in mappings)
                {
                    if (mappings.Length == 1 && Guid.TryParse(filenameWithoutExtension, out Guid guidFromFilename))
                    {
                        ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mapping, guidFromFilename, path);
                    }
                    else
                    {
                        ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mapping, null, path);
                    }
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
                    Guid? guid = ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mappingModels[0]);
                    return ResponseMessageBuilder.Create("Mapping added", 201, guid);
                }

                foreach (var mappingModel in mappingModels)
                {
                    ConvertWireMockOrgMappingAndRegisterAsRespondProvider(mappingModel);
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

        private Guid? ConvertWireMockOrgMappingAndRegisterAsRespondProvider(OrgMapping mapping, Guid? guid = null, string path = null)
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
            if (guid != null)
            {
                respondProvider = respondProvider.WithGuid(guid.Value);
            }
            else if (!string.IsNullOrEmpty(mapping.Uuid))
            {
                respondProvider = respondProvider.WithGuid(new Guid(mapping.Uuid));
            }

            if (mapping.Name != null)
            {
                respondProvider = respondProvider.WithTitle(mapping.Name);
            }

            if (path != null)
            {
                respondProvider = respondProvider.WithPath(path);
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

                var matcher = ProcessAsStringMatcher(match, valueAsString);
                if (matcher != null)
                {
                    action(key, matcher);
                }
            }
        }

        private void ProcessWireMockOrgJObjectAndUseIMatcher(JObject items, Action<IMatcher> action)
        {
            IMatcher matcher = null;

            var firstItem = items.First as JProperty;

            if (firstItem?.Name == "equalToJson")
            {
                matcher = new JsonMatcher(firstItem.Value);
            }
            else
            {
                var valueAsString = (firstItem.Value as JValue)?.Value as string;
                if (valueAsString == null)
                {
                    return;
                }

                matcher = ProcessAsStringMatcher(firstItem, valueAsString);
            }

            if (matcher != null)
            {
                action(matcher);
            }
        }

        private static IStringMatcher ProcessAsStringMatcher(JProperty match, string valueAsString)
        {
            switch (match?.Name)
            {
                case "contains":
                    return new WildcardMatcher(valueAsString);

                case "matches":
                    return new RegexMatcher(valueAsString);

                case "equalTo":
                    return new ExactMatcher(valueAsString);

                default:
                    return null;
            }
        }
    }
}
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

            // string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            if (FileHelper.TryReadMappingFileWithRetryAndDelay(_settings.FileSystemHandler, path, out string value))
            {
                var mappings = DeserializeJsonToArray<OrgMapping>(value);
                foreach (var mapping in mappings)
                {
                    Map(mapping);
                }
            }
        }

        private void Map(OrgMapping mapping)
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
                  "equalToJson" : "{ \"cityName\": \"São Paulo\", \"cityCode\": 5001 }",
                  "ignoreArrayOrder" : true,
                  "ignoreExtraElements" : true
                } ]
                */
                if (request.BodyPatterns?.Any() == true)
                {
                    var jObjectArray = request.BodyPatterns.Cast<JObject>();
                    var bodyPattern = jObjectArray.First();
                    ProcessWireMockOrgJObjectAndUseValueMatcher(bodyPattern, (match) =>
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

                if (response.Headers is IDictionary<string, string> responseHeaders)
                {
                    responseBuilder = responseBuilder.WithHeaders(responseHeaders);
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
            }

            /*
             * {
        *      "id" : "365dd908-dc67-4f27-9e41-15d908206d81",
        *      "name" : "weatherforecast_register-city",
        *      "request" : {
        *        "url" : "/WeatherForecast/register-city",
        *       "method" : "POST",
                "bodyPatterns" : [ {
                  "equalToJson" : "{ \"cityName\": \"São Paulo\", \"cityCode\": 5001 }",
                  "ignoreArrayOrder" : true,
                  "ignoreExtraElements" : true
                } ]
              },
              "response" : {
         *       "status" : 200,
         *       "headers" : {
         *         "Date" : "Wed, 08 Sep 2021 23:48:33 GMT",
         *         "Server" : "Kestrel"
                }
              },
              "uuid" : "365dd908-dc67-4f27-9e41-15d908206d81",
              "persistent" : true,
              "insertionIndex" : 4
            }
             */
        }

        private void ProcessWireMockOrgJObjectAndUseStringMatcher(JObject items, Action<string, IStringMatcher> action)
        {
            foreach (var item in items)
            {
                var key = item.Key;
                var match = item.Value.First as JProperty;

                IStringMatcher matcher = null;
                switch (match?.Name)
                {
                    case "contains":
                    case "equalTo":
                        matcher = new WildcardMatcher(match.Value.Value<string>());
                        break;
                }

                if (matcher != null)
                {
                    action(key, matcher);
                }
            }
        }

        private void ProcessWireMockOrgJObjectAndUseValueMatcher(JObject items, Action<IValueMatcher> action)
        {
            IValueMatcher matcher = null;
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
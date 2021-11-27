using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Net.OpenApiParser.Utils;

namespace WireMock.Net.OpenApiParser.Mappers
{
    internal class OpenApiPathsMapper
    {
        private const string HeaderContentType = "Content-Type";

        private readonly WireMockOpenApiParserSettings _settings;
        private readonly ExampleValueGenerator _exampleValueGenerator;

        public OpenApiPathsMapper(WireMockOpenApiParserSettings settings)
        {
            _settings = Guard.NotNull(settings, nameof(settings));
            _exampleValueGenerator = new ExampleValueGenerator(settings);
        }

        public IEnumerable<MappingModel> ToMappingModels(OpenApiPaths paths, IList<OpenApiServer> servers)
        {
            return paths.Select(p => MapPath(p.Key, p.Value, servers)).SelectMany(x => x);
        }

        private IEnumerable<MappingModel> MapPaths(OpenApiPaths paths, IList<OpenApiServer> servers)
        {
            return paths.Select(p => MapPath(p.Key, p.Value, servers)).SelectMany(x => x);
        }

        private IEnumerable<MappingModel> MapPath(string path, OpenApiPathItem pathItem, IList<OpenApiServer> servers)
        {
            return pathItem.Operations.Select(o => MapOperationToMappingModel(path, o.Key.ToString().ToUpperInvariant(), o.Value, servers));
        }

        private MappingModel MapOperationToMappingModel(string path, string httpMethod, OpenApiOperation operation, IList<OpenApiServer> servers)
        {
            var queryParameters = operation.Parameters.Where(p => p.In == ParameterLocation.Query);
            var pathParameters = operation.Parameters.Where(p => p.In == ParameterLocation.Path);
            var headers = operation.Parameters.Where(p => p.In == ParameterLocation.Header);

            var response = operation.Responses.FirstOrDefault();

            TryGetContent(response.Value?.Content, out OpenApiMediaType responseContent, out string responseContentType);
            var responseSchema = response.Value?.Content?.FirstOrDefault().Value?.Schema;
            var responseExample = responseContent?.Example;
            var responseSchemaExample = responseContent?.Schema?.Example;

            var body = responseExample != null ? MapOpenApiAnyToJToken(responseExample) :
                                   responseSchemaExample != null ? MapOpenApiAnyToJToken(responseSchemaExample) :
                                   MapSchemaToObject(responseSchema);

            var requestBodyModel = new BodyModel();
            if (operation.RequestBody != null && operation.RequestBody.Content != null && operation.RequestBody.Required)
            {
                var request = operation.RequestBody.Content;
                TryGetContent(request, out OpenApiMediaType requestContent, out string requestContentType);

                var requestBodySchema = operation.RequestBody.Content.First().Value?.Schema;
                var requestBodyExample = requestContent.Example;
                var requestBodySchemaExample = requestContent.Schema?.Example;

                var requestBodyMapped = requestBodyExample != null ? MapOpenApiAnyToJToken(requestBodyExample) :
                                   requestBodySchemaExample != null ? MapOpenApiAnyToJToken(requestBodySchemaExample) :
                                   MapSchemaToObject(requestBodySchema);

                requestBodyModel = MapRequestBody(requestBodyMapped);
            }

            if (!int.TryParse(response.Key, out var httpStatusCode))
            {
                httpStatusCode = 200;
            }

            return new MappingModel
            {
                Guid = Guid.NewGuid(),
                Request = new RequestModel
                {
                    Methods = new[] { httpMethod },
                    Path = MapBasePath(servers) + MapPathWithParameters(path, pathParameters),
                    Params = MapQueryParameters(queryParameters),
                    Headers = MapRequestHeaders(headers),
                    Body = requestBodyModel
                },
                Response = new ResponseModel
                {
                    StatusCode = httpStatusCode,
                    Headers = MapHeaders(responseContentType, response.Value?.Headers),
                    BodyAsJson = body
                }
            };
        }

        private BodyModel MapRequestBody(object requestBody)
        {
            if (requestBody == null)
            {
                return null;
            }

            var requestBodyModel = new BodyModel();
            requestBodyModel.Matcher = new MatcherModel();
            requestBodyModel.Matcher.Name = "JsonMatcher";
            requestBodyModel.Matcher.Pattern = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
            requestBodyModel.Matcher.IgnoreCase = true;
            return requestBodyModel;
        }

        private bool TryGetContent(IDictionary<string, OpenApiMediaType> contents, out OpenApiMediaType openApiMediaType, out string contentType)
        {
            openApiMediaType = null;
            contentType = null;

            if (contents == null || contents.Values.Count == 0)
            {
                return false;
            }

            if (contents.TryGetValue("application/json", out var content))
            {
                openApiMediaType = content;
                contentType = "application/json";
            }
            else
            {
                var first = contents.FirstOrDefault();
                openApiMediaType = first.Value;
                contentType = first.Key;
            }

            return true;
        }

        private object MapSchemaToObject(OpenApiSchema schema, string name = null)
        {
            if (schema == null)
            {
                return null;
            }

            switch (schema.GetSchemaType())
            {
                case SchemaType.Array:
                    var jArray = new JArray();
                    for (int i = 0; i < _settings.NumberOfArrayItems; i++)
                    {
                        if (schema.Items.Properties.Count > 0)
                        {
                            var arrayItem = new JObject();
                            foreach (var property in schema.Items.Properties)
                            {
                                var objectValue = MapSchemaToObject(property.Value, property.Key);
                                if (objectValue is JProperty jp)
                                {
                                    arrayItem.Add(jp);
                                }
                                else
                                {
                                    arrayItem.Add(new JProperty(property.Key, objectValue));
                                }
                            }

                            jArray.Add(arrayItem);
                        }
                        else
                        {
                            jArray.Add(MapSchemaToObject(schema.Items, name));
                        }
                    }

                    if (schema.AllOf.Count > 0)
                    {
                        jArray.Add(MapSchemaAllOfToObject(schema));
                    }

                    return jArray;

                case SchemaType.Boolean:
                case SchemaType.Integer:
                case SchemaType.Number:
                case SchemaType.String:
                    return _exampleValueGenerator.GetExampleValue(schema);

                case SchemaType.Object:
                    var propertyAsJObject = new JObject();
                    foreach (var schemaProperty in schema.Properties)
                    {
                        propertyAsJObject.Add(MapPropertyAsJObject(schemaProperty.Value, schemaProperty.Key));
                    }
                    if (schema.AllOf.Count > 0)
                    {
                        foreach (var property in schema.AllOf)
                        {
                            foreach (var item in property.Properties)
                            {
                                propertyAsJObject.Add(MapPropertyAsJObject(item.Value, item.Key));
                            }
                        }
                    }

                    return name != null ? new JProperty(name, propertyAsJObject) : (JToken)propertyAsJObject;

                default:
                    return null;
            }
        }

        private JObject MapSchemaAllOfToObject(OpenApiSchema schema)
        {
            var arrayItem = new JObject();
            foreach (var property in schema.AllOf)
            {
                foreach (var item in property.Properties)
                {
                    arrayItem.Add(MapPropertyAsJObject(item.Value, item.Key));
                }
            }
            return arrayItem;
        }

        private object MapPropertyAsJObject(OpenApiSchema openApiSchema, string key)
        {
            if (openApiSchema.GetSchemaType() == SchemaType.Object || openApiSchema.GetSchemaType() == SchemaType.Array)
            {
                var mapped = MapSchemaToObject(openApiSchema, key);
                if (mapped is JProperty jp)
                {
                    return jp;
                }
                else
                {
                    return new JProperty(key, mapped);
                }
            }
            else
            {
                // bool propertyIsNullable = openApiSchema.Nullable || (openApiSchema.TryGetXNullable(out bool x) && x);
                return new JProperty(key, _exampleValueGenerator.GetExampleValue(openApiSchema));
            }
        }

        private string MapPathWithParameters(string path, IEnumerable<OpenApiParameter> parameters)
        {
            if (parameters == null)
            {
                return path;
            }

            string newPath = path;
            foreach (var parameter in parameters)
            {
                var exampleMatcherModel = GetExampleMatcherModel(parameter.Schema, _settings.PathPatternToUse);
                newPath = newPath.Replace($"{{{parameter.Name}}}", exampleMatcherModel.Pattern as string);
            }

            return newPath;
        }

        private string MapBasePath(IList<OpenApiServer> servers)
        {
            if (servers == null || servers.Count == 0)
            {
                return string.Empty;
            }

            OpenApiServer server = servers.First();
            if (Uri.TryCreate(server.Url, UriKind.RelativeOrAbsolute, out Uri uriResult))
            {
                return uriResult.IsAbsoluteUri ? uriResult.AbsolutePath : uriResult.ToString();
            }

            return string.Empty;
        }

        private JToken MapOpenApiAnyToJToken(IOpenApiAny any)
        {
            if (any == null)
            {
                return null;
            }

            using var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);
            any.Write(writer, OpenApiSpecVersion.OpenApi3_0);

            if (any.AnyType == AnyType.Array)
            {
                return JArray.Parse(outputString.ToString());
            }
            else
            {
                return JObject.Parse(outputString.ToString());
            }
        }

        private IDictionary<string, object> MapHeaders(string responseContentType, IDictionary<string, OpenApiHeader> headers)
        {
            var mappedHeaders = headers.ToDictionary(
                item => item.Key,
                item => GetExampleMatcherModel(null, _settings.HeaderPatternToUse).Pattern
            );

            if (!string.IsNullOrEmpty(responseContentType))
            {
                mappedHeaders.TryAdd(HeaderContentType, responseContentType);
            }

            return mappedHeaders.Keys.Any() ? mappedHeaders : null;
        }

        private IList<ParamModel> MapQueryParameters(IEnumerable<OpenApiParameter> queryParameters)
        {
            var list = queryParameters
                .Where(req => req.Required)
                .Select(qp => new ParamModel
                {
                    Name = qp.Name,
                    IgnoreCase = true,
                    Matchers = new[]
                    {
                        GetExampleMatcherModel(qp.Schema, _settings.QueryParameterPatternToUse)
                    }
                })
                .ToList();

            return list.Any() ? list : null;
        }

        private IList<HeaderModel> MapRequestHeaders(IEnumerable<OpenApiParameter> headers)
        {
            var list = headers
                .Where(req => req.Required)
                .Select(qp => new HeaderModel
                {
                    Name = qp.Name,
                    IgnoreCase = true,
                    Matchers = new[]
                    {
                        GetExampleMatcherModel(qp.Schema, _settings.HeaderPatternToUse)
                    }
                })
                .ToList();

            return list.Any() ? list : null;
        }

        private MatcherModel GetExampleMatcherModel(OpenApiSchema schema, ExampleValueType type)
        {
            return type switch
            {
                ExampleValueType.Value => new MatcherModel { Name = "ExactMatcher", Pattern = GetExampleValueAsStringForSchemaType(schema), IgnoreCase = true },

                _ => new MatcherModel { Name = "WildcardMatcher", Pattern = "*" }
            };
        }

        private string GetExampleValueAsStringForSchemaType(OpenApiSchema schema)
        {
            var value = _exampleValueGenerator.GetExampleValue(schema);

            return value switch
            {
                string valueAsString => valueAsString,

                _ => value.ToString(),
            };
        }
    }
}
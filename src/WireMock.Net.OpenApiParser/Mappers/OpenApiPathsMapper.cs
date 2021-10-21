using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Net.OpenApiParser.Utils;

namespace WireMock.Net.OpenApiParser.Mappers
{
    internal class OpenApiPathsMapper
    {
        private readonly WireMockOpenApiParserSettings _settings;
        private readonly ExampleValueGenerator _exampleValueGenerator;

        public OpenApiPathsMapper(WireMockOpenApiParserSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _exampleValueGenerator = new ExampleValueGenerator(settings);
        }

        public IEnumerable<MappingModel> ToMappingModels(OpenApiPaths paths)
        {
            return paths.Select(p => MapPath(p.Key, p.Value)).SelectMany(x => x);
        }

        private IEnumerable<MappingModel> MapPaths(OpenApiPaths paths)
        {
            return paths.Select(p => MapPath(p.Key, p.Value)).SelectMany(x => x);
        }

        private IEnumerable<MappingModel> MapPath(string path, OpenApiPathItem pathItem)
        {
            return pathItem.Operations.Select(o => MapOperationToMappingModel(path, o.Key.ToString().ToUpperInvariant(), o.Value));
        }

        private MappingModel MapOperationToMappingModel(string path, string httpMethod, OpenApiOperation operation)
        {
            var queryParameters = operation.Parameters.Where(p => p.In == ParameterLocation.Query);
            var pathParameters = operation.Parameters.Where(p => p.In == ParameterLocation.Path);
            var headers = operation.Parameters.Where(p => p.In == ParameterLocation.Header);

            var response = operation.Responses.FirstOrDefault();

            TryGetContent(response.Value?.Content, out OpenApiMediaType responseContent, out string responseContentType);
            var responseSchema = response.Value?.Content?.FirstOrDefault().Value?.Schema;
            var responseExample = responseContent?.Example;

            var body = responseExample != null ? MapOpenApiAnyToJToken(responseExample) : MapSchemaToObject(responseSchema);

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
                    Path = MapPathWithParameters(path, pathParameters),
                    Params = MapQueryParameters(queryParameters),
                    Headers = MapRequestHeaders(headers)
                },
                Response = new ResponseModel
                {
                    StatusCode = httpStatusCode,
                    Headers = MapHeaders(responseContentType, response.Value?.Headers),
                    BodyAsJson = body
                }
            };
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
                        string propertyName = schemaProperty.Key;
                        var openApiSchema = schemaProperty.Value;
                        if (openApiSchema.GetSchemaType() == SchemaType.Object || openApiSchema.GetSchemaType() == SchemaType.Array)
                        {
                            var mapped = MapSchemaToObject(schemaProperty.Value, schemaProperty.Key);
                            if (mapped is JProperty jp)
                            {
                                propertyAsJObject.Add(jp);
                            }
                            else
                            {
                                propertyAsJObject.Add(new JProperty(schemaProperty.Key, mapped));
                            }
                        }
                        else
                        {
                            bool propertyIsNullable = openApiSchema.Nullable || (openApiSchema.TryGetXNullable(out bool x) && x);

                            propertyAsJObject.Add(new JProperty(propertyName, _exampleValueGenerator.GetExampleValue(openApiSchema)));
                        }
                    }

                    return name != null ? new JProperty(name, propertyAsJObject) : (JToken)propertyAsJObject;

                default:
                    return null;
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
                newPath = newPath.Replace($"{{{parameter.Name}}}", GetExampleValue(parameter.Schema, _settings.PathPatternToUse));
            }

            return newPath;
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
                item => GetExampleValue(null, _settings.HeaderPatternToUse) as object
            );

            if (!string.IsNullOrEmpty(responseContentType))
            {
                if (!mappedHeaders.ContainsKey("Content-Type"))
                {
                    mappedHeaders.Add("Content-Type", responseContentType);
                }
                else
                {
                    mappedHeaders["Content-Type"] = responseContentType;
                }
            }

            return mappedHeaders.Keys.Any() ? mappedHeaders : null;
        }

        private IList<ParamModel> MapQueryParameters(IEnumerable<OpenApiParameter> queryParameters)
        {
            var list = queryParameters
                .Select(qp => new ParamModel
                {
                    Name = qp.Name,
                    Matchers = new[]
                    {
                        new MatcherModel
                        {
                            Name = "ExactMatcher",
                            Pattern = GetDefaultValueAsStringForSchemaType(qp.Schema)
                        }
                    }
                })
                .ToList();

            return list.Any() ? list : null;
        }

        private IList<HeaderModel> MapRequestHeaders(IEnumerable<OpenApiParameter> headers)
        {
            var list = headers
                .Select(qp => new HeaderModel
                {
                    Name = qp.Name,
                    Matchers = new[]
                    {
                        new MatcherModel
                        {
                            Name = "ExactMatcher",
                            Pattern = GetDefaultValueAsStringForSchemaType(qp.Schema)
                        }
                    }
                })
                .ToList();

            return list.Any() ? list : null;
        }

        private string GetDefaultValueAsStringForSchemaType(OpenApiSchema schema)
        {
            var value = _exampleValueGenerator.GetExampleValue(schema);

            switch (value)
            {
                case string valueAsString:
                    return valueAsString;

                default:
                    return value.ToString();
            }
        }

        private string GetExampleValue(OpenApiSchema schema, ExampleValueType type)
        {
            switch (type)
            {
                case ExampleValueType.Value:
                    return GetDefaultValueAsStringForSchemaType(schema);

                default:
                    return "*";
            }
        }
    }
}
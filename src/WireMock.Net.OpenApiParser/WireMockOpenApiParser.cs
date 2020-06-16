using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using RamlToOpenApiConverter;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Net.OpenApiParser.Utils;

namespace WireMock.Net.OpenApiParser
{
    /// <summary>
    /// Parse a OpenApi/Swagger/V2/V3 or Raml to WireMock MappingModels.
    /// </summary>
    public class WireMockOpenApiParser : IWireMockOpenApiParser
    {
        private const int ArrayItems = 3;

        private readonly OpenApiStreamReader _reader = new OpenApiStreamReader();

        public IEnumerable<MappingModel> FromFile(string path, out OpenApiDiagnostic diagnostic)
        {
            OpenApiDocument document;
            if (Path.GetExtension(path).EndsWith("raml", StringComparison.OrdinalIgnoreCase))
            {
                diagnostic = new OpenApiDiagnostic();
                document = new RamlConverter().ConvertToOpenApiDocument(path);
            }
            else
            {
                var reader = new OpenApiStreamReader();
                document = reader.Read(File.OpenRead(path), out diagnostic);
            }

            return FromDocument(document);
        }

        public IEnumerable<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic)
        {
            return FromDocument(_reader.Read(stream, out diagnostic));
        }

        public IEnumerable<MappingModel> FromDocument(OpenApiDocument openApiDocument)
        {
            return MapPaths(openApiDocument.Paths);
        }

        private static IEnumerable<MappingModel> MapPaths(OpenApiPaths paths)
        {
            return paths.Select(p => MapPath(p.Key, p.Value)).SelectMany(x => x);
        }

        private static IEnumerable<MappingModel> MapPath(string path, OpenApiPathItem pathItem)
        {
            return pathItem.Operations.Select(o => MapOperationToMappingModel(path, o.Key.ToString().ToUpperInvariant(), o.Value));
        }

        private static MappingModel MapOperationToMappingModel(string path, string httpMethod, OpenApiOperation operation)
        {
            var queryParameters = operation.Parameters.Where(p => p.In == ParameterLocation.Query);
            var pathParameters = operation.Parameters.Where(p => p.In == ParameterLocation.Path);
            var response = operation.Responses.FirstOrDefault();
            TryGetContent(response.Value?.Content, out OpenApiMediaType responseContent, out string responseContentType);
            var responseSchema = response.Value?.Content?.FirstOrDefault().Value?.Schema;
            var responseExample = responseContent?.Example;

            var body = responseExample != null ? MapOpenApiAnyToJToken(responseExample) : MapSchemaToObject(responseSchema);

            if (int.TryParse(response.Key, out var httpStatusCode))
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
                    Params = MapQueryParameters(queryParameters)
                },
                Response = new ResponseModel
                {
                    StatusCode = httpStatusCode,
                    Headers = MapHeaders(responseContentType, response.Value?.Headers),
                    BodyAsJson = body
                }
            };
        }

        private static bool TryGetContent(IDictionary<string, OpenApiMediaType> contents, out OpenApiMediaType openApiMediaType, out string contentType)
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

        private static object MapSchemaToObject(OpenApiSchema schema, string name = null)
        {
            if (schema == null)
            {
                return null;
            }

            switch (schema.GetSchemaType())
            {
                case SchemaType.Array:
                    var jArray = new JArray();
                    for (int i = 0; i < ArrayItems; i++)
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
                    return ExampleValueGenerator.GetExampleValue(schema);

                case SchemaType.Object:
                    var propertyAsJObject = new JObject();
                    foreach (var schemaProperty in schema.Properties)
                    {
                        string propertyName = schemaProperty.Key;
                        var openApiSchema = schemaProperty.Value;
                        if (openApiSchema.GetSchemaType() == SchemaType.Object)
                        {
                            var mapped = MapSchemaToObject(schemaProperty.Value, schemaProperty.Key);
                            if (mapped is JProperty jp)
                            {
                                propertyAsJObject.Add(jp);
                            }
                        }
                        else
                        {
                            bool propertyIsNullable = openApiSchema.Nullable || openApiSchema.TryGetXNullable(out bool x) && x;

                            propertyAsJObject.Add(new JProperty(propertyName, ExampleValueGenerator.GetExampleValue(openApiSchema)));
                        }
                    }

                    return name != null ? new JProperty(name, propertyAsJObject) : (JToken)propertyAsJObject;

                default:
                    return null;
            }
        }

        private static string MapPathWithParameters(string path, IEnumerable<OpenApiParameter> parameters)
        {
            if (parameters == null)
            {
                return path;
            }

            string newPath = path;
            foreach (var parameter in parameters)
            {
                newPath = newPath.Replace($"{{{parameter.Name}}}", ExampleValueGenerator.GetExampleValue(parameter.Schema).ToString());
            }

            return newPath;
        }

        private static JToken MapOpenApiAnyToJToken(IOpenApiAny any)
        {
            if (any == null)
            {
                return null;
            }

            using (var outputString = new StringWriter())
            {
                var writer = new OpenApiJsonWriter(outputString);
                any.Write(writer, OpenApiSpecVersion.OpenApi3_0);

                return JObject.Parse(outputString.ToString());
            }
        }

        private static IDictionary<string, object> MapHeaders(string responseContentType, IDictionary<string, OpenApiHeader> headers)
        {
            var mappedHeaders = headers.ToDictionary(item => item.Key, item => ExampleValueGenerator.GetExampleValue(null));
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

        private static IList<ParamModel> MapQueryParameters(IEnumerable<OpenApiParameter> queryParameters)
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

        private static string GetDefaultValueAsStringForSchemaType(OpenApiSchema schema)
        {
            var value = ExampleValueGenerator.GetExampleValue(schema);

            switch (value)
            {
                case string valueAsString:
                    return valueAsString;

                default:
                    return value.ToString();
            }
        }
    }
}
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Net.OpenApiParser.Utils;

namespace WireMock.Net.OpenApiParser
{
    public class WireMockOpenApiParser : IWireMockOpenApiParser
    {
        private const int ArrayItems = 3;

        private readonly OpenApiStreamReader _reader = new OpenApiStreamReader();

        public IEnumerable<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic)
        {
            var openApiDocument = _reader.Read(stream, out diagnostic);

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
            var content = response.Value?.Content?.FirstOrDefault().Value;
            var schema = response.Value?.Content?.FirstOrDefault().Value?.Schema;
            var example = content?.Example;

            var body = example != null ? MapOpenApiAnyToJToken(example) : MapSchemaToObject(schema);

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
                    Headers = MapHeaders(response.Value?.Headers),
                    BodyAsJson = body
                }
            };
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
                                arrayItem.Add(new JProperty(property.Key, objectValue));
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
                    return GetDefaultValueForSchemaType(schema);

                case SchemaType.String:
                    return GetDefaultValueForSchemaType(schema);

                case SchemaType.Object:
                    var propertyAsJObject = new JObject();
                    foreach (var schemaProperty in schema.Properties)
                    {
                        var mapped = MapSchemaToObject(schemaProperty.Value, schemaProperty.Key);
                        if (mapped != null)
                        {
                            var add = name != null ? new JProperty(name, mapped) : new JProperty(schemaProperty.Key, mapped);
                            propertyAsJObject.Add(add);
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
                newPath = newPath.Replace($"{{{parameter.Name}}}", GetDefaultValueForSchemaType(parameter.Schema).ToString());
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

        private static IDictionary<string, object> MapHeaders(IDictionary<string, OpenApiHeader> headers)
        {
            var mappedHeaders = headers.ToDictionary(item => item.Key, item => (object)GetDefaultValueForSchemaType(null));

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
            var value = GetDefaultValueForSchemaType(schema);

            switch (value)
            {
                case string valueAsString:
                    return valueAsString;

                default:
                    return value.ToString();
            }
        }

        private static object GetDefaultValueForSchemaType(OpenApiSchema schema)
        {
            switch (schema?.GetSchemaType())
            {
                case SchemaType.Integer:
                    return 42;

                case SchemaType.Number:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Float:
                            return 4.2f;

                        default:
                            return 4.2d;
                    }

                case SchemaType.Boolean:
                    return true;

                default:
                    switch (schema?.GetSchemaFormat())
                    {
                        case SchemaFormat.Date:
                            return DateTimeUtils.ToRfc3339Date(DateTime.UtcNow);

                        case SchemaFormat.DateTime:
                            return DateTimeUtils.ToRfc3339DateTime(DateTime.UtcNow);

                        case SchemaFormat.Byte:
                            return new byte[] { 48 };

                        case SchemaFormat.Binary:
                            return "example-object";

                        default:
                            return "example-string";
                    }
            }
        }
    }
}
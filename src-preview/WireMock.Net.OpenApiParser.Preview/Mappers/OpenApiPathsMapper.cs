// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Extensions;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Net.OpenApiParser.Types;
using WireMock.Net.OpenApiParser.Utils;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace WireMock.Net.OpenApiParser.Mappers;

internal class OpenApiPathsMapper
{
    private const string HeaderContentType = "Content-Type";

    private readonly WireMockOpenApiParserSettings _settings;
    private readonly ExampleValueGenerator _exampleValueGenerator;

    public OpenApiPathsMapper(WireMockOpenApiParserSettings settings)
    {
        _settings = Guard.NotNull(settings);
        _exampleValueGenerator = new ExampleValueGenerator(settings);
    }

    public IReadOnlyList<MappingModel> ToMappingModels(OpenApiPaths? paths, IList<OpenApiServer> servers)
    {
        return paths?
            .OrderBy(p => p.Key)
            .Select(p => MapPath(p.Key, p.Value, servers))
            .SelectMany(x => x)
            .ToArray() ?? [];
    }

    private IReadOnlyList<MappingModel> MapPath(string path, IOpenApiPathItem pathItem, IList<OpenApiServer> servers)
    {
        return pathItem.Operations?.Select(o => MapOperationToMappingModel(path, o.Key.ToString().ToUpperInvariant(), o.Value, servers)).ToArray() ?? [];
    }

    private MappingModel MapOperationToMappingModel(string path, string httpMethod, OpenApiOperation operation, IList<OpenApiServer> servers)
    {
        var queryParameters = operation.Parameters?.Where(p => p.In == ParameterLocation.Query) ?? [];
        var pathParameters = operation.Parameters?.Where(p => p.In == ParameterLocation.Path) ?? [];
        var headers = operation.Parameters?.Where(p => p.In == ParameterLocation.Header) ?? [];

        var response = operation?.Responses?.FirstOrDefault() ?? new KeyValuePair<string, IOpenApiResponse>();

        TryGetContent(response.Value?.Content, out OpenApiMediaType? responseContent, out var responseContentType);
        var responseSchema = response.Value?.Content?.FirstOrDefault().Value?.Schema;
        var responseExample = responseContent?.Example;
        var responseSchemaExample = responseContent?.Schema?.Example;

        var responseBody = responseExample ?? responseSchemaExample ?? MapSchemaToObject(responseSchema);

        var requestBodyModel = new BodyModel();
        if (operation.RequestBody != null && operation.RequestBody.Content != null && operation.RequestBody.Required)
        {
            var request = operation.RequestBody.Content;
            TryGetContent(request, out var requestContent, out _);

            var requestBodySchema = operation.RequestBody.Content.First().Value?.Schema;
            var requestBodyExample = requestContent!.Example;
            var requestBodySchemaExample = requestContent.Schema?.Example;

            var requestBodyMapped = requestBodyExample ?? requestBodySchemaExample ?? MapSchemaToObject(requestBodySchema);
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
                Methods = [httpMethod],
                Path = PathUtils.Combine(MapBasePath(servers), MapPathWithParameters(path, pathParameters)),
                Params = MapQueryParameters(queryParameters),
                Headers = MapRequestHeaders(headers),
                Body = requestBodyModel
            },
            Response = new ResponseModel
            {
                StatusCode = httpStatusCode,
                Headers = MapHeaders(responseContentType, response.Value?.Headers),
                BodyAsJson = responseBody != null ? JsonConvert.DeserializeObject(SystemTextJsonSerializer.Serialize(responseBody)) : null
            }
        };
    }

    private BodyModel? MapRequestBody(JsonNode? requestBody)
    {
        if (requestBody == null)
        {
            return null;
        }

        return new BodyModel
        {
            Matcher = new MatcherModel
            {
                Name = "JsonMatcher",
                Pattern = SystemTextJsonSerializer.Serialize(requestBody, new JsonSerializerOptions { WriteIndented = true }),
                IgnoreCase = _settings.RequestBodyIgnoreCase
            }
        };
    }

    private static bool TryGetContent(IDictionary<string, OpenApiMediaType>? contents, [NotNullWhen(true)] out OpenApiMediaType? openApiMediaType, [NotNullWhen(true)] out string? contentType)
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

    private JsonNode? MapSchemaToObject(IOpenApiSchema? schema)
    {
        if (schema == null)
        {
            return null;
        }

        switch (schema.GetSchemaType(out _))
        {
            case JsonSchemaType.Array:
                var array = new JsonArray();
                for (var i = 0; i < _settings.NumberOfArrayItems; i++)
                {
                    if (schema.Items?.Properties?.Count > 0)
                    {
                        var item = new JsonObject();
                        foreach (var property in schema.Items.Properties)
                        {
                            item[property.Key] = MapSchemaToObject(property.Value);
                        }

                        array.Add(item);
                    }
                    else
                    {
                        var arrayItem = MapSchemaToObject(schema.Items);
                        array.Add(arrayItem);
                    }
                }

                if (schema.AllOf?.Count > 0)
                {
                    array.Add(MapSchemaAllOfToObject(schema));
                }

                return array;

            case JsonSchemaType.Boolean:
            case JsonSchemaType.Integer:
            case JsonSchemaType.Number:
            case JsonSchemaType.String:
                return _exampleValueGenerator.GetExampleValue(schema);

            case JsonSchemaType.Object:
                var propertyAsJsonObject = new JsonObject();
                foreach (var schemaProperty in schema.Properties ?? new Dictionary<string, IOpenApiSchema>())
                {
                    propertyAsJsonObject[schemaProperty.Key] = MapPropertyAsJsonNode(schemaProperty.Value);
                }

                if (schema.AllOf?.Count > 0)
                {
                    foreach (var group in schema.AllOf.SelectMany(p => p.Properties ?? new Dictionary<string, IOpenApiSchema>()).GroupBy(x => x.Key))
                    {
                        propertyAsJsonObject[group.Key] = MapPropertyAsJsonNode(group.First().Value);
                    }
                }

                return propertyAsJsonObject;

            default:
                return null;
        }
    }

    private JsonObject MapSchemaAllOfToObject(IOpenApiSchema schema)
    {
        var arrayItem = new JsonObject();
        foreach (var property in schema.AllOf ?? [])
        {
            foreach (var item in property.Properties ?? new Dictionary<string, IOpenApiSchema>())
            {
                arrayItem[item.Key] = MapPropertyAsJsonNode(item.Value);
            }
        }
        return arrayItem;
    }

    private JsonNode? MapPropertyAsJsonNode(IOpenApiSchema openApiSchema)
    {
        var schemaType = openApiSchema.GetSchemaType(out _);
        if (schemaType is JsonSchemaType.Object or JsonSchemaType.Array)
        {
            return MapSchemaToObject(openApiSchema);
        }

        return _exampleValueGenerator.GetExampleValue(openApiSchema);
    }

    private string MapPathWithParameters(string path, IEnumerable<IOpenApiParameter>? parameters)
    {
        if (parameters == null)
        {
            return path;
        }

        var newPath = path;
        foreach (var parameter in parameters)
        {
            var exampleMatcherModel = GetExampleMatcherModel(parameter.Schema, _settings.PathPatternToUse);
            newPath = newPath.Replace($"{{{parameter.Name}}}", exampleMatcherModel.Pattern as string);
        }

        return newPath;
    }

    private IDictionary<string, object>? MapHeaders(string? responseContentType, IDictionary<string, IOpenApiHeader>? headers)
    {
        var mappedHeaders = headers?
            .ToDictionary(item => item.Key, _ => GetExampleMatcherModel(null, _settings.HeaderPatternToUse).Pattern!) ?? new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(responseContentType))
        {
            mappedHeaders.TryAdd(HeaderContentType, responseContentType);
        }

        return mappedHeaders.Keys.Any() ? mappedHeaders : null;
    }

    private IList<ParamModel>? MapQueryParameters(IEnumerable<IOpenApiParameter> queryParameters)
    {
        var list = queryParameters
            .Where(req => req.Required)
            .Select(qp => new ParamModel
            {
                Name = qp.Name ?? string.Empty,
                IgnoreCase = _settings.QueryParameterPatternIgnoreCase,
                Matchers =
                [
                    GetExampleMatcherModel(qp.Schema, _settings.QueryParameterPatternToUse)
                ]
            })
            .ToList();

        return list.Any() ? list : null;
    }

    private IList<HeaderModel>? MapRequestHeaders(IEnumerable<IOpenApiParameter> headers)
    {
        var list = headers
            .Where(req => req.Required)
            .Select(qp => new HeaderModel
            {
                Name = qp.Name ?? string.Empty,
                IgnoreCase = _settings.HeaderPatternIgnoreCase,
                Matchers =
                [
                    GetExampleMatcherModel(qp.Schema, _settings.HeaderPatternToUse)
                ]
            })
            .ToList();

        return list.Any() ? list : null;
    }

    private MatcherModel GetExampleMatcherModel(IOpenApiSchema? schema, ExampleValueType type)
    {
        return type switch
        {
            ExampleValueType.Value => new MatcherModel
            {
                Name = "ExactMatcher",
                Pattern = GetExampleValueAsStringForSchemaType(schema),
                IgnoreCase = _settings.IgnoreCaseExampleValues
            },

            _ => new MatcherModel
            {
                Name = "WildcardMatcher",
                Pattern = "*"
            }
        };
    }

    private string GetExampleValueAsStringForSchemaType(IOpenApiSchema? schema)
    {
        var value = _exampleValueGenerator.GetExampleValue(schema);

        if (value.GetValueKind() == JsonValueKind.String)
        {
            return value.GetValue<string>();
        }

        return value.ToString();
    }

    private static string MapBasePath(IList<OpenApiServer>? servers)
    {
        var server = servers?.FirstOrDefault();
        if (server == null)
        {
            return string.Empty;
        }

        if (Uri.TryCreate(server.Url, UriKind.RelativeOrAbsolute, out var uriResult))
        {
            return uriResult.IsAbsoluteUri ? uriResult.AbsolutePath : uriResult.ToString();
        }

        return string.Empty;
    }
}
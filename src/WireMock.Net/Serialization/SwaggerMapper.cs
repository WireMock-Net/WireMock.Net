using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using WireMock.Admin.Mappings;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Server;

namespace WireMock.Serialization;

internal static class SwaggerMapper
{
    private const string DefaultMethod = "GET";

    private const string DefaultContentType = "application/json";

    public static string ToSwagger(WireMockServer server)
    {
        var openApiDocument = new OpenApiDocument();

        foreach (var mapping in server.MappingModels)
        {
            var path = mapping.Request.GetPathAsString();
            if (path == null)
            {
                // Path is null (probably a Func<>), skip this.
                continue;
            }

            var operation = new OpenApiOperation
            {
                RequestBody = MapRequest(mapping.Request)
            };

            var statusCode = mapping.Response.GetStatusCodeAsString();

            var response = MapResponse(mapping.Response);
            if (response != null)
            {
                operation.Responses.Add(statusCode, response);
            }

            var method = mapping.Request.Methods?.FirstOrDefault() ?? DefaultMethod;
            if (!openApiDocument.Paths.ContainsKey(path))
            {
                var openApiPathItem = new OpenApiPathItem
                {
                    { method, operation }
                };

                openApiDocument.Paths.Add(path, openApiPathItem);
            }
            else
            {
                // The combination of path+method uniquely identify an operation. Duplicates are not allowed.
                if (!openApiDocument.Paths[path].ContainsKey(method))
                {
                    openApiDocument.Paths[path].Add(method, operation);
                }
            }
        }

        return openApiDocument.ToJson(SchemaType.OpenApi3, Formatting.Indented);
    }

    private static OpenApiRequestBody? MapRequest(RequestModel request)
    {
        var body = MapBody(request.Body);
        if (body == null)
        {
            return null;
        }

        var openApiMediaType = new OpenApiMediaType
        {
            Schema = JsonSchema.FromType(body.GetType())
        };
        openApiMediaType.Schema.Description = null;
        openApiMediaType.Schema.Title = null;

        var requestBodyPost = new OpenApiRequestBody();
        requestBodyPost.Content.Add(GetContentType(request), openApiMediaType);

        return requestBodyPost;
    }

    private static OpenApiResponse? MapResponse(ResponseModel response)
    {
        if (response.Body != null)
        {
            try
            {
                var x = JsonConvert.DeserializeObject(response.Body);
                CreateOpenApiResponse(x);
            }
            catch
            {
                // Ignore and continue
            }

            var openApiResponseAsString = new OpenApiResponse
            {
                Schema = new JsonSchemaProperty
                {
                    Type = JsonObjectType.String,
                    Example = response.Body
                }
            };

            return openApiResponseAsString;
        }

        if (response.BodyAsJson == null)
        {
            return null;
        }

        return CreateOpenApiResponse(response.BodyAsJson);
    }

    private static OpenApiResponse CreateOpenApiResponse(object bodyAsJson)
    {
        var openApiResponse = new OpenApiResponse
        {
            Schema = JsonSchema.FromType(bodyAsJson.GetType())
        };
        openApiResponse.Schema.Description = null;
        openApiResponse.Schema.Title = null;

        return openApiResponse;
    }

    private static object? MapBody(BodyModel? body)
    {
        if (body == null || body.Matcher == null || body.Matchers == null)
        {
            return null;
        }

        if (body.Matcher is { Name: nameof(JsonMatcher) })
        {
            return body.Matcher.Pattern;
        }

        var jsonMatcher = body.Matchers.FirstOrDefault(m => m.Name == nameof(JsonMatcher));
        return jsonMatcher?.Pattern;
    }

    private static string GetContentType(RequestModel request)
    {
        var contentType = request.Headers?.FirstOrDefault(h => h.Name == "Content-Type");

        return contentType == null ?
            DefaultContentType :
            GetPatternAsStringFromMatchers(contentType.Matchers, DefaultContentType);
    }

    private static string GetPatternAsStringFromMatchers(IList<MatcherModel>? matchers, string defaultValue)
    {
        if (matchers != null && matchers.Any() && matchers.First().Pattern is string patternAsString)
        {
            return patternAsString;
        }

        return defaultValue;
    }
}
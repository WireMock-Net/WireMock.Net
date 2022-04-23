using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using WireMock.Admin.Mappings;
using WireMock.Constants;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Server;

namespace WireMock.Serialization;

internal static class SwaggerMapper
{
    private const string DefaultMethod = "GET";

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

            var operation = new OpenApiOperation();
            foreach (var openApiParameter in MapRequestQueryParameters(mapping.Request.Params))
            {
                operation.Parameters.Add(openApiParameter);
            }
            foreach (var openApiParameter in MapRequestHeaders(mapping.Request.Headers))
            {
                operation.Parameters.Add(openApiParameter);
            }
            foreach (var openApiParameter in MapRequestCookies(mapping.Request.Cookies))
            {
                operation.Parameters.Add(openApiParameter);
            }

            operation.RequestBody = MapRequestBody(mapping.Request);

            var response = MapResponse(mapping.Response);
            if (response != null)
            {
                operation.Responses.Add(mapping.Response.GetStatusCodeAsString(), response);
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

    private static IEnumerable<OpenApiParameter> MapRequestQueryParameters(IList<ParamModel>? queryParameters)
    {
        if (queryParameters == null)
        {
            return new List<OpenApiParameter>();
        }

        return queryParameters
            .Where(x => x.Matchers != null && x.Matchers.Any())
            .Select(x => new
            {
                x.Name,
                X = GetDetailsFromMatcher(x.Matchers![0])
            })
            .Select(x => new OpenApiParameter
            {
                Name = x.Name,
                Pattern = x.X.RegexPattern,
                Example = x.X.Example,
                Description = x.X.Description,
                Kind = OpenApiParameterKind.Query
            })
            .ToList();
    }

    private static (string? RegexPattern, string? Example, string? Description) GetDetailsFromMatcher(MatcherModel matcher)
    {
        var pattern = GetPatternAsStringFromMatcher(matcher);
        var description = $"{matcher.Name} (RejectOnMatch = {matcher.RejectOnMatch == true})";

        if (matcher.Name is nameof(RegexMatcher) or nameof(WildcardMatcher) or nameof(ExactMatcher))
        {
            return (pattern, pattern, description);
        }

        return (null, pattern, description);
    }

    private static IEnumerable<OpenApiParameter> MapRequestHeaders(IList<HeaderModel>? headers)
    {
        if (headers == null)
        {
            return new List<OpenApiParameter>();
        }

        return headers
            .Where(x => x.Matchers != null && x.Matchers.Any())
            .Select(x => new
            {
                x.Name,
                X = GetDetailsFromMatcher(x.Matchers![0])
            })
            .Select(x => new OpenApiParameter
            {
                Name = x.Name,
                Pattern = x.X.RegexPattern,
                Example = x.X.Example,
                Description = x.X.Description,
                Kind = OpenApiParameterKind.Header
            })
            .ToList();
    }

    private static IEnumerable<OpenApiParameter> MapRequestCookies(IList<CookieModel>? cookies)
    {
        if (cookies == null)
        {
            return new List<OpenApiParameter>();
        }

        return cookies
            .Where(x => x.Matchers != null && x.Matchers.Any())
            .Select(x => new
            {
                x.Name,
                X = GetDetailsFromMatcher(x.Matchers![0])
            })
            .Select(x => new OpenApiParameter
            {
                Name = x.Name,
                Pattern = x.X.RegexPattern,
                Example = x.X.Example,
                Description = x.X.Description,
                Kind = OpenApiParameterKind.Cookie
            })
            .ToList();
    }

    private static OpenApiRequestBody? MapRequestBody(RequestModel request)
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
            return new OpenApiResponse
            {
                Schema = new JsonSchemaProperty
                {
                    Type = JsonObjectType.String,
                    Example = response.Body
                }
            };
        }

        if (response.BodyAsBytes != null)
        {
            // https://stackoverflow.com/questions/62794949/how-to-define-byte-array-in-openapi-3-0
            return new OpenApiResponse
            {
                Schema = new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Items =
                    {
                        new JsonSchema
                        {
                            Type = JsonObjectType.String,
                            Format = "byte"
                        }
                    }
                }
            };
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
            Schema = JsonSchema.FromType(bodyAsJson.GetType(), new JsonSchemaGeneratorSettings
            {
                GenerateCustomNullableProperties = false
            })
        };
        openApiResponse.Schema.Description = null;
        openApiResponse.Schema.Title = null;

        // Remove "null" from the type.
        // Example:
        //
        // "result": {
        //   "type": [
        //     "null",
        //     "string"
        //   ]
        // }
        foreach (var property in openApiResponse.Schema.Properties.Where(p => p.Value.Type.HasFlag(JsonObjectType.Null)))
        {
            property.Value.Type &= ~JsonObjectType.Null;
        }

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
            WireMockConstants.ContentTypeJson :
            GetPatternAsStringFromMatchers(contentType.Matchers, WireMockConstants.ContentTypeJson);
    }

    private static string GetPatternAsStringFromMatchers(IList<MatcherModel>? matchers, string defaultValue)
    {
        if (matchers == null || !matchers.Any())
        {
            return defaultValue;
        }

        return GetPatternAsStringFromMatcher(matchers.First()) ?? defaultValue;
    }

    private static string? GetPatternAsStringFromMatcher(MatcherModel matcher)
    {
        if (matcher.Pattern is string patternAsString)
        {
            return patternAsString;
        }

        return matcher.Patterns?.FirstOrDefault() as string;
    }
}
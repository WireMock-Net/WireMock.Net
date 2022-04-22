using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using WireMock.Admin.Mappings;
using WireMock.Server;

namespace WireMock.Serialization;

internal static class SwaggerMapper
{
    private const string DefaultMethod = "GET";
    private const string DefaultContentType = "application/json";

    public static string ToSwagger(WireMockServer server)
    {
        var doc = new OpenApiDocument();

        var p = new OpenApiPathItem();
        foreach (var mapping in server.MappingModels)
        {
            p.Add(mapping.Request.Methods?.FirstOrDefault() ?? DefaultMethod, new OpenApiOperation
            {
                RequestBody = MapRequest(mapping.Request)
                //Responses =
                //{
                //    new KeyValuePair<string, OpenApiResponse>("200", responsePost)
                //}
            });
        }



        //var requestPost = new OpenApiMediaType();
        //requestPost.Schema = JsonSchema.FromType(o.GetType());
        //requestPost.Schema.Title = null;

        //var requestBodyPost = new OpenApiRequestBody();
        //requestBodyPost.Content.Add("application/json", requestPost);

        //var responsePost = new OpenApiResponse();
        //responsePost.Schema = JsonSchema.FromType(o.GetType());
        //responsePost.Schema.Title = null;

        doc.Paths.Add("/pet", p);

        return doc.ToJson(SchemaType.OpenApi3, Formatting.Indented);
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
        openApiMediaType.Schema.Title = null;

        var requestBodyPost = new OpenApiRequestBody();
        requestBodyPost.Content.Add(GetContentType(request), openApiMediaType);

        return requestBodyPost;
    }

    private static object? MapBody(BodyModel? body)
    {
        if (body == null || body.Matcher.Name != "JsonMatcher")
        {
            return null;
        }

        return body.Matcher.Pattern;
    }

    private static string GetContentType(RequestModel request)
    {
        var contentType = request.Headers?.FirstOrDefault(h => h.Name == "Content-Type");
        if (contentType == null)
        {
            return DefaultContentType;
        }

        return GetPatternAsStringFromMatchers(contentType.Matchers, DefaultContentType);
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
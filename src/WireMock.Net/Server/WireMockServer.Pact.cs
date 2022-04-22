using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Pact.Models.V2;
using WireMock.Util;

namespace WireMock.Server;

public partial class WireMockServer
{
    private const string DefaultPath = "/";
    private const string DefaultMethod = "GET";
    private const int DefaultStatus = 200;
    private const string DefaultConsumer = "Default Consumer";
    private const string DefaultProvider = "Default Provider";

    /// <summary>
    /// Save the mappings as a Pact Json file V2.
    /// </summary>
    /// <param name="folder">The folder to save the pact file.</param>
    /// <param name="filename">The filename for the .json file [optional].</param>
    public void SavePact(string folder, string? filename = null)
    {
        var consumer = Consumer ?? DefaultConsumer;
        var provider = Provider ?? DefaultProvider;

        filename ??= $"{consumer} - {provider}.json";

        var pact = new Pact.Models.V2.Pact
        {
            Consumer = new Pacticipant { Name = consumer },
            Provider = new Pacticipant { Name = provider }
        };

        foreach (var mapping in MappingModels)
        {
            var interaction = new Interaction
            {
                Description = mapping.Description,
                ProviderState = mapping.Title,
                Request = MapRequest(mapping.Request),
                Response = MapResponse(mapping.Response)
            };

            pact.Interactions.Add(interaction);
        }

        var bytes = JsonUtils.SerializeAsPactFile(pact);
        _settings.FileSystemHandler.WriteFile(folder, filename, bytes);
    }

    private static Request MapRequest(RequestModel request)
    {
        string path;
        switch (request.Path)
        {
            case string pathAsString:
                path = pathAsString;
                break;

            case PathModel pathModel:
                path = GetPatternAsStringFromMatchers(pathModel.Matchers, DefaultPath);
                break;

            default:
                path = DefaultPath;
                break;
        }

        return new Request
        {
            Method = request.Methods?.FirstOrDefault() ?? DefaultMethod,
            Path = path,
            Query = MapQueryParameters(request.Params),
            Headers = MapRequestHeaders(request.Headers),
            Body = MapBody(request.Body)
        };
    }

    private static Response MapResponse(ResponseModel? response)
    {
        if (response == null)
        {
            return new Response();
        }

        return new Response
        {
            Status = MapStatusCode(response.StatusCode),
            Headers = MapResponseHeaders(response.Headers),
            Body = response.BodyAsJson
        };
    }

    private static int MapStatusCode(object? statusCode)
    {
        if (statusCode is string statusCodeAsString)
        {
            return int.TryParse(statusCodeAsString, out var statusCodeAsInt) ? statusCodeAsInt : DefaultStatus;
        }

        if (statusCode != null)
        {
            // Convert to Int32 because Newtonsoft deserializes an 'object' with a number value to a long.
            return Convert.ToInt32(statusCode);
        }

        return DefaultStatus;
    }

    private static string? MapQueryParameters(IList<ParamModel>? queryParameters)
    {
        if (queryParameters == null)
        {
            return null;
        }

        var values = queryParameters
            .Where(qp => qp.Matchers != null && qp.Matchers.Any() && qp.Matchers[0].Pattern is string)
            .Select(param => $"{Uri.EscapeDataString(param.Name)}={Uri.EscapeDataString((string)param.Matchers![0].Pattern)}");

        return string.Join("&", values);
    }

    private static IDictionary<string, string>? MapRequestHeaders(IList<HeaderModel>? headers)
    {
        if (headers == null)
        {
            return null;
        }

        var validHeaders = headers.Where(h => h.Matchers != null && h.Matchers.Any() && h.Matchers[0].Pattern is string);
        return validHeaders.ToDictionary(x => x.Name, y => (string)y.Matchers![0].Pattern);
    }

    private static IDictionary<string, string>? MapResponseHeaders(IDictionary<string, object>? headers)
    {
        if (headers == null)
        {
            return null;
        }

        var validHeaders = headers.Where(h => h.Value is string);
        return validHeaders.ToDictionary(x => x.Key, y => (string)y.Value);
    }

    private static object? MapBody(BodyModel? body)
    {
        if (body == null || body.Matcher.Name != "JsonMatcher")
        {
            return null;
        }

        return body.Matcher.Pattern;
    }

    private static string GetPatternAsStringFromMatchers(MatcherModel[]? matchers, string defaultValue)
    {
        if (matchers != null && matchers.Any() && matchers[0].Pattern is string patternAsString)
        {
            return patternAsString;
        }

        return defaultValue;
    }
}
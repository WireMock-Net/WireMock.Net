using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Net.Pact.Models.V2;
using WireMock.Server;

namespace WireMock.Net.Pact.Extensions;

public static class WireMockServerExtensions
{
    public static void SavePact(this WireMockServer server, string? folder = null)
    {
        var pact = new Models.V2.Pact
        {
            Consumer = new Pacticipant { Name = server.Consumer },
            Provider = new Pacticipant { Name = server.Provider }
        };

        foreach (var mapping in server.MappingModels)
        {
            var interaction = new Interaction
            {
                Description = mapping.Description,
                ProviderState = mapping.Title,
                Request = MapRequest(mapping.Request)
            };

            pact.Interactions.Add(interaction);
        }
    }

    private static Request MapRequest(RequestModel request)
    {
        return new Request
        {
            Method = request.Methods.FirstOrDefault() ?? "GET",
            Path = request.Path as string ?? "/",
            Query = MapQueryParameters(request.Params),
            Headers = MapHeaders(request.Headers),
            Body = MapBody(request.Body)
        };
    }

    private static string? MapQueryParameters(IList<ParamModel>? queryParameters)
    {
        if (queryParameters == null)
        {
            return null;
        }

        var values = new List<string>();
        foreach (var param in queryParameters.Where(qp => qp.Matchers.Any() && qp.Matchers[0].Pattern is string))
        {
            values.Add($"{Uri.EscapeDataString(param.Name)}={Uri.EscapeDataString((string)param.Matchers[0].Pattern)}");
        }

        return string.Join("&", values);
    }

    private static IDictionary<string, string>? MapHeaders(IList<HeaderModel> headers)
    {
        if (!headers.Any())
        {
            return null;
        }

        var validHeaders = headers.Where(h => h.Matchers.Any() && h.Matchers[0].Pattern is string);
        return validHeaders.ToDictionary(x => x.Name, y => y.Matchers[0].Pattern as string ?? string.Empty);
    }

    private static object? MapBody(BodyModel? body)
    {
        if (body == null || body.Matcher.Name != "JsonMatcher")
        {
            return null;
        }

        return body.Matcher.Pattern;
    }
}
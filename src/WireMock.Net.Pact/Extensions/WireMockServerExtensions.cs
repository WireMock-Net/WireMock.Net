using System.Collections.Generic;
using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Net.Pact.Models;
using WireMock.Server;

namespace WireMock.Net.Pact.Extensions;

public static class WireMockServerExtensions
{
    public static void SavePact(this WireMockServer server, string? folder = null)
    {
        var pact = new Models.Pact
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
                Request = Map(mapping.Request)
            };

            pact.Interactions.Add(interaction);
        }
    }

    private static Request Map(RequestModel request)
    {
        return new Request
        {
            Method = request.Methods.FirstOrDefault() ?? "GET",
            Path = request.Path as string ?? "/",
            Headers = Map(request.Headers)
        };
    }

    private static IDictionary<string, string>? Map(IList<HeaderModel> headers)
    {
        if (!headers.Any())
        {
            return null;
        }

        var validHeaders = headers.Where(h => h.Matchers.Any() && h.Matchers[0].Pattern is string);
        return validHeaders.ToDictionary(x => x.Name, y => y.Matchers[0].Pattern as string ?? string.Empty);
    }
}
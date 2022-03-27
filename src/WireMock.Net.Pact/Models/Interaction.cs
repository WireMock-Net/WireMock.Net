using System.Collections.Generic;

namespace WireMock.Net.Pact.Models;

public class Interaction
{
    public string? Description { get; set; }

    public List<ProviderState> ProviderStates { get; set; } = new();

    public Request Request { get; set; }

    public Response Response { get; set; }
}
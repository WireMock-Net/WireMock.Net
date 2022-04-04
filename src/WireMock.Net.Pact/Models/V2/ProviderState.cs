using System.Collections.Generic;

namespace WireMock.Net.Pact.Models.V2;

public class ProviderState
{
    public string Name { get; set; } = null!;

    public IDictionary<string, string>? Params { get; set; }
}
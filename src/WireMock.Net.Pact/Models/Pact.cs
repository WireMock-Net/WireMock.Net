using System.Collections.Generic;

namespace WireMock.Net.Pact.Models;

public class Pact
{
    public Consumer? Consumer { get; set; }

    public List<Interaction> Interactions { get; set; } = new();

    public Metadata? Metadata { get; set; }

    public Provider? Provider { get; set; }
}
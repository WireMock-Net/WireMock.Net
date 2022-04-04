using System.Collections.Generic;

namespace WireMock.Net.Pact.Models.V2;

public class Pact
{
    public Pacticipant? Consumer { get; set; }

    public List<Interaction> Interactions { get; set; } = new();

    public Metadata? Metadata { get; set; }

    public Pacticipant? Provider { get; set; }
}
using System;

namespace WireMock.Net.Pact.Models;

public class Pact
{
    public Pacticipant? Consumer { get; set; }

    public Interaction[] Interactions { get; set; } = Array.Empty<Interaction>();

    public Metadata? Metadata { get; set; }

    public Pacticipant? Provider { get; set; }
}
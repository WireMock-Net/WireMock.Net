using System;
using System.Collections.Generic;

namespace WireMock.Net.Pact.Models;

public class Pact
{
    public Pacticipant? Consumer { get; set; }

    public List<Interaction> Interactions { get; set; } = new();

    public Metadata? Metadata { get; set; }

    public Pacticipant? Provider { get; set; }
}
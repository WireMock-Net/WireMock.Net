// Copyright © WireMock.Net

#pragma warning disable CS1591
using System.Collections.Generic;

namespace WireMock.Pact.Models.V2;

public class Pact
{
    public Pacticipant Consumer { get; set; }

    public List<Interaction> Interactions { get; set; } = new List<Interaction>();

    public Metadata Metadata { get; set; }

    public Pacticipant Provider { get; set; }
}
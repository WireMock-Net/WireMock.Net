// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class Interaction
{
    public string? Description { get; set; }

    public string? ProviderState { get; set; }

    public PactRequest Request { get; set; } = new();

    public PactResponse Response { get; set; } = new();
}
namespace WireMock.Pact.Models.V2;

public class Interaction
{
    public string Description { get; set; } = string.Empty;

    public string ProviderState { get; set; } = string.Empty;

    public PactRequest Request { get; set; } = new();

    public PactResponse Response { get; set; } = new();
}
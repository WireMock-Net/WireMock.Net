namespace WireMock.Pact.Models.V2;

public class Interaction
{
    public string Description { get; set; } = string.Empty;

    public string ProviderState { get; set; }

    public PactRequest Request { get; set; } = new PactRequest();

    public PactResponse Response { get; set; } = new PactResponse();
}
namespace WireMock.Net.Pact.Models;

public class Metadata
{
    public PactRust? PactRust { get; set; }

    public PactSpecification PactSpecification { get; set; } = new();
}
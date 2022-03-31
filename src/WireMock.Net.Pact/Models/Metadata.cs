namespace WireMock.Net.Pact.Models;

public class Metadata
{
    public string? PactSpecificationVersion { get; set; }

    public PactSpecification PactSpecification { get; set; } = new();
}
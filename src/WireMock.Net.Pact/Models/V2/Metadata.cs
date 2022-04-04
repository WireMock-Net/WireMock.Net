namespace WireMock.Net.Pact.Models.V2;

public class Metadata
{
    public string? PactSpecificationVersion { get; set; }

    public PactSpecification PactSpecification { get; set; } = new();
}
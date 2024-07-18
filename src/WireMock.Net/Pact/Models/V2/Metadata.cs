// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class Metadata
{
    public string PactSpecificationVersion { get; set; }

    public PactSpecification PactSpecification { get; set; } = new PactSpecification();
}
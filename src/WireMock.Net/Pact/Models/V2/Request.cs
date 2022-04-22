using System.Collections.Generic;

namespace WireMock.Pact.Models.V2;

public class Request
{
    public IDictionary<string, string>? Headers { get; set; }

    public string Method { get; set; } = "GET";

    public string? Path { get; set; } = "/";

    public string? Query { get; set; }

    public object? Body { get; set; }
}
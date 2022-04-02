using System.Collections.Generic;

namespace WireMock.Net.Pact.Models;

public class Request
{
    public IDictionary<string, string>? Headers { get; set; }

    public string Method { get; set; } = "GET";

    public string Path { get; set; } = "/";

    public List<MatchingRule>? MatchingRules;
}
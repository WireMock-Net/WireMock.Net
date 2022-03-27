using System.Collections.Generic;

namespace WireMock.Net.Pact.Models;

public class Response
{
    public object? Body { get; set; }

    public IDictionary<string, string>? Headers { get; set; }

    public int Status { get; set; } = 200;
}
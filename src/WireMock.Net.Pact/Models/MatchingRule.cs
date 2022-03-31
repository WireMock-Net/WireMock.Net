namespace WireMock.Net.Pact.Models;

public class MatchingRule
{
    /// <summary>
    /// type or regex
    /// </summary>
    public string Match { get; set; } = "type";

    /// <summary>
    /// Used for Match = "type"
    /// </summary>
    public string? Min { get; set; }

    /// <summary>
    /// Used for Match = "type"
    /// </summary>
    public string? Max { get; set; }

    /// <summary>
    /// Used for Match = "regex"
    /// </summary>
    public string? Regex { get; set; }
}
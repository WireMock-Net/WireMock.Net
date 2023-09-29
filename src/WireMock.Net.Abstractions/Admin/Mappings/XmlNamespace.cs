namespace WireMock.Admin.Mappings;

/// <summary>
/// Defines an xml namespace consisting of prefix and uri.
/// <example>xmlns:i="http://www.w3.org/2001/XMLSchema-instance"</example>
/// </summary>
public class XmlNamespace
{
    /// <summary>
    /// The prefix.
    /// <example>i</example>
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// The uri.
    /// <example>http://www.w3.org/2001/XMLSchema-instance</example>
    /// </summary>
    public string Uri { get; set; }
}
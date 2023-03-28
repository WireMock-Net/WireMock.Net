using WireMock.Net.OpenApiParser.Abstractions.Settings;

namespace WireMock.Net.OpenApiParser.Abstractions.Types;

/// <summary>
/// The example value to use
/// </summary>
public enum ExampleValueType
{
    /// <summary>
    /// 1. Use a generated example value based on the SchemaType (default).
    /// 2. If there is no example value defined in the schema,
    ///    then the <see cref="IWireMockOpenApiParserExampleValues"/> will be used (custom, fixed or dynamic).
    /// </summary>
    Value,

    /// <summary>
    /// Just use a Wildcard (*) character.
    /// </summary>
    Wildcard
}
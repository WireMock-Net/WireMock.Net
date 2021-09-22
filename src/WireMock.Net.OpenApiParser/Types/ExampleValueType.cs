namespace WireMock.Net.OpenApiParser.Types
{
    /// <summary>
    /// The example value to use
    /// </summary>
    public enum ExampleValueType
    {
        /// <summary>
        /// Use a generated example value based on the SchemaType (default).
        /// </summary>
        Value,

        /// <summary>
        /// Just use a Wildcard (*) character.
        /// </summary>
        Wildcard
    }
}
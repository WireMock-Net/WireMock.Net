using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Settings
{
    /// <summary>
    /// The WireMockOpenApiParser Settings
    /// </summary>
    public class WireMockOpenApiParserSettings
    {
        /// <summary>
        /// The number of array items to generate (default is 3).
        /// </summary>
        public int NumberOfArrayItems { get; set; } = 3;

        /// <summary>
        /// The example value type to use when generating a Path
        /// </summary>
        public ExampleValueType PathPatternToUse { get; set; } = ExampleValueType.Value;

        /// <summary>
        /// The example value type to use when generating a Header
        /// </summary>
        public ExampleValueType HeaderPatternToUse { get; set; } = ExampleValueType.Value;

        /// <summary>
        /// The example value type to use when generating a Query Parameter
        /// </summary>
        public ExampleValueType QueryParameterPatternToUse { get; set; } = ExampleValueType.Value;

        /// <summary>
        /// The example values to use
        /// </summary>
        public IWireMockOpenApiParserExampleValues ExampleValues { get; set; }

        /// <summary>
        /// Are examples generated dynamically?
        /// </summary>
        public bool DynamicExamples { get; set; } = false;

        /// <summary>
        /// Is headers case sensitive? (default is true).
        /// </summary>
        public bool IgnoreCaseHeaders { get; set; } = true;

        /// <summary>
        /// Is query params case sensitive? (default is true).
        /// </summary>
        public bool IgnoreCaseQueryParams { get; set; } = true;

        /// <summary>
        /// Is request body case sensitive? (default is true).
        /// </summary>
        public bool IgnoreCaseRequestBody { get; set; } = true;

        /// <summary>
        /// Are example values case sensitive? (default is true).                
        /// </summary>
        public bool IgnoreCaseExampleValues { get; set; } = true;
    }
}
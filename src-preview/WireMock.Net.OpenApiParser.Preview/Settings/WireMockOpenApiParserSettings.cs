// Copyright © WireMock.Net

using WireMock.Net.OpenApiParser.Types;

namespace WireMock.Net.OpenApiParser.Settings;

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
    /// The example values to use.
    ///
    /// Default implementations are:
    /// - <see cref="WireMockOpenApiParserExampleValues"/>
    /// - <see cref="WireMockOpenApiParserDynamicExampleValues"/>
    /// </summary>
    public IWireMockOpenApiParserExampleValues? ExampleValues { get; set; }

    /// <summary>
    /// Is a Header match case-insensitive?
    /// 
    /// Default is <c>true</c>.
    /// </summary>
    public bool HeaderPatternIgnoreCase { get; set; } = true;

    /// <summary>
    /// Is a Query Parameter match case-insensitive?
    ///
    /// Default is <c>true</c>.
    /// </summary>
    public bool QueryParameterPatternIgnoreCase { get; set; } = true;

    /// <summary>
    /// Is a Request Body match case-insensitive?
    ///
    /// Default is <c>true</c>.
    /// </summary>
    public bool RequestBodyIgnoreCase { get; set; } = true;

    /// <summary>
    /// Is a ExampleValue match case-insensitive?
    ///
    /// Default is <c>true</c>.
    /// </summary>
    public bool IgnoreCaseExampleValues { get; set; } = true;

    /// <summary>
    /// Are examples generated dynamically?
    /// </summary>
    public bool DynamicExamples { get; set; }
}
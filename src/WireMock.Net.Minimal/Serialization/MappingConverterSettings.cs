// Copyright © WireMock.Net

using WireMock.Types;

namespace WireMock.Serialization;

/// <summary>
/// MappingConverterSettings
/// </summary>
public class MappingConverterSettings
{
    /// <summary>
    /// Use 'Server' or 'Builder'.
    ///
    /// Default is Server
    /// </summary>
    public MappingConverterType ConverterType { get; set; }

    /// <summary>
    /// Add "var server = WireMockServer.Start();"
    /// or
    /// Add "var builder = new MappingBuilder();"
    ///
    /// Default it's false.
    /// </summary>
    public bool AddStart { get; set; }
}
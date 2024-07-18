// Copyright © WireMock.Net

using System;
using Microsoft.OpenApi.Models;

namespace WireMock.Net.OpenApiParser.Settings;

/// <summary>
/// A interface defining the example values to use for the different types.
/// </summary>
public interface IWireMockOpenApiParserExampleValues
{
    /// <summary>
    /// An example value for a Boolean.
    /// </summary>
    bool Boolean { get; }

    /// <summary>
    /// An example value for an Integer.
    /// </summary>
    int Integer { get; }

    /// <summary>
    /// An example value for a Float.
    /// </summary>
    float Float { get; }

    /// <summary>
    /// An example value for a Double.
    /// </summary>
    double Double { get; }

    /// <summary>
    /// An example value for a Date.
    /// </summary>
    Func<DateTime> Date { get; }

    /// <summary>
    /// An example value for a DateTime.
    /// </summary>
    Func<DateTime> DateTime { get; }

    /// <summary>
    /// An example value for Bytes.
    /// </summary>
    byte[] Bytes { get; }

    /// <summary>
    /// An example value for a Object.
    /// </summary>
    object Object { get; }

    /// <summary>
    /// An example value for a String.
    /// </summary>
    string String { get; }

    /// <summary>
    /// OpenApi Schema to generate dynamic examples more accurate
    /// </summary>
    OpenApiSchema? Schema { get; set; }
}
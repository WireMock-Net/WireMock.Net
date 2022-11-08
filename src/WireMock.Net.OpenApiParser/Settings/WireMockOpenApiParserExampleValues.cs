using System;
using Microsoft.OpenApi.Models;

namespace WireMock.Net.OpenApiParser.Settings;

/// <summary>
/// A class defining the example values to use for the different types.
/// </summary>
public class WireMockOpenApiParserExampleValues : IWireMockOpenApiParserExampleValues
{
    /// <inheritdoc />
    public virtual bool Boolean { get; set; } = true;

    /// <inheritdoc />
    public virtual int Integer { get; set; } = 42;

    /// <inheritdoc />
    public virtual float Float { get; set; } = 4.2f;

    /// <inheritdoc />
    public virtual double Double { get; set; } = 4.2d;

    /// <inheritdoc />
    public virtual Func<DateTime> Date { get; set; } = () => System.DateTime.UtcNow.Date;

    /// <inheritdoc />
    public virtual Func<DateTime> DateTime { get; set; } = () => System.DateTime.UtcNow;

    /// <inheritdoc />
    public virtual byte[] Bytes { get; set; } = { 48, 49, 50 };

    /// <inheritdoc />
    public virtual object Object { get; set; } = "example-object";

    /// <inheritdoc />
    public virtual string String { get; set; } = "example-string";

    /// <inheritdoc />
    public virtual OpenApiSchema? Schema { get; set; } = new OpenApiSchema();
}
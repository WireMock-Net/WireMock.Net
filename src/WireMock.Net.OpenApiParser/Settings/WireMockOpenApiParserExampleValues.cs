// Copyright © WireMock.Net

using System;
using Microsoft.OpenApi.Models;

namespace WireMock.Net.OpenApiParser.Settings;

/// <summary>
/// A class defining the example values to use for the different types.
/// </summary>
public class WireMockOpenApiParserExampleValues : IWireMockOpenApiParserExampleValues
{
    /// <inheritdoc />
    public virtual bool Boolean => true;

    /// <inheritdoc />
    public virtual int Integer => 42;

    /// <inheritdoc />
    public virtual float Float => 4.2f;

    /// <inheritdoc />
    public virtual double Double => 4.2d;

    /// <inheritdoc />
    public virtual Func<DateTime> Date { get; } = () => System.DateTime.UtcNow.Date;

    /// <inheritdoc />
    public virtual Func<DateTime> DateTime { get; } = () => System.DateTime.UtcNow;

    /// <inheritdoc />
    public virtual byte[] Bytes { get; } = { 48, 49, 50 };

    /// <inheritdoc />
    public virtual object Object => "example-object";

    /// <inheritdoc />
    public virtual string String => "example-string";

    /// <inheritdoc />
    public virtual OpenApiSchema? Schema { get; set; } = new();
}
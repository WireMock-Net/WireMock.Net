// Copyright © WireMock.Net

using System;
using Microsoft.OpenApi.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace WireMock.Net.OpenApiParser.Settings;

/// <summary>
/// A class defining the random example values to use for the different types.
/// </summary>
public class WireMockOpenApiParserDynamicExampleValues : IWireMockOpenApiParserExampleValues
{
    /// <inheritdoc />
    public virtual bool Boolean => RandomizerFactory.GetRandomizer(new FieldOptionsBoolean()).Generate() ?? true;

    /// <inheritdoc />
    public virtual int Integer => RandomizerFactory.GetRandomizer(new FieldOptionsInteger()).Generate() ?? 42;

    /// <inheritdoc />
    public virtual float Float => RandomizerFactory.GetRandomizer(new FieldOptionsFloat()).Generate() ?? 4.2f;

    /// <inheritdoc />
    public virtual double Double => RandomizerFactory.GetRandomizer(new FieldOptionsDouble()).Generate() ?? 4.2d;

    /// <inheritdoc />
    public virtual Func<DateTime> Date => () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow.Date;

    /// <inheritdoc />
    public virtual Func<DateTime> DateTime => () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow;

    /// <inheritdoc />
    public virtual byte[] Bytes => RandomizerFactory.GetRandomizer(new FieldOptionsBytes()).Generate();

    /// <inheritdoc />
    public virtual object Object => "example-object";

    /// <inheritdoc />
    public virtual string String => RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[0-9]{2}[A-Z]{5}[0-9]{2}" }).Generate() ?? "example-string";

    /// <inheritdoc />
    public virtual OpenApiSchema? Schema { get; set; }
}
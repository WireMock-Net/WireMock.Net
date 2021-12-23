using System;
using Microsoft.OpenApi.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace WireMock.Net.OpenApiParser.Settings
{
    /// <summary>
    /// A class defining the random example values to use for the different types.
    /// </summary>
    public class WireMockOpenApiParserDynamicExampleValues : IWireMockOpenApiParserExampleValues
    {
        /// <inheritdoc />
        public bool Boolean { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBoolean()).Generate() ?? true; } set { } }
        /// <inheritdoc />
        public int Integer { get { return RandomizerFactory.GetRandomizer(new FieldOptionsInteger()).Generate() ?? 42; } set { } }
        /// <inheritdoc />
        public float Float { get { return RandomizerFactory.GetRandomizer(new FieldOptionsFloat()).Generate() ?? 4.2f; } set { } }
        /// <inheritdoc />
        public double Double { get { return RandomizerFactory.GetRandomizer(new FieldOptionsDouble()).Generate() ?? 4.2d; } set { } }
        /// <inheritdoc />
        public Func<DateTime> Date { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow.Date; } set { } }
        /// <inheritdoc />
        public Func<DateTime> DateTime { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow; } set { } }
        /// <inheritdoc />
        public byte[] Bytes { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBytes()).Generate(); } set { } }
        /// <inheritdoc />
        public object Object { get; set; } = "example-object";
        /// <inheritdoc />
        public string String { get { return RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[0-9]{2}[A-Z]{5}[0-9]{2}" }).Generate() ?? "example-string"; } set { } }
        /// <inheritdoc />
        public OpenApiSchema SchemaExample { get; set; }
    }
}
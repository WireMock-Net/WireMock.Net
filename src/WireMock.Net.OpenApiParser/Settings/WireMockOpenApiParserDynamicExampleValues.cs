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
        public virtual bool Boolean { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBoolean()).Generate() ?? true; } set { } }
        /// <inheritdoc />
        public virtual int Integer { get { return RandomizerFactory.GetRandomizer(new FieldOptionsInteger()).Generate() ?? 42; } set { } }
        /// <inheritdoc />
        public virtual float Float { get { return RandomizerFactory.GetRandomizer(new FieldOptionsFloat()).Generate() ?? 4.2f; } set { } }
        /// <inheritdoc />
        public virtual double Double { get { return RandomizerFactory.GetRandomizer(new FieldOptionsDouble()).Generate() ?? 4.2d; } set { } }
        /// <inheritdoc />
        public virtual Func<DateTime> Date { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow.Date; } set { } }
        /// <inheritdoc />
        public virtual Func<DateTime> DateTime { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow; } set { } }
        /// <inheritdoc />
        public virtual byte[] Bytes { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBytes()).Generate(); } set { } }
        /// <inheritdoc />
        public virtual object Object { get; set; } = "example-object";
        /// <inheritdoc />
        public virtual string String { get { return RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[0-9]{2}[A-Z]{5}[0-9]{2}" }).Generate() ?? "example-string"; } set { } }
        /// <inheritdoc />
        public virtual OpenApiSchema Schema { get; set; }
    }
}
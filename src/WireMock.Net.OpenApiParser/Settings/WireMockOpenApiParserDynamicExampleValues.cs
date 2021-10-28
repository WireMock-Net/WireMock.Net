using System;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace WireMock.Net.OpenApiParser.Settings
{
    /// <summary>
    /// A class defining the example values to use for the different types.
    /// </summary>
    public class WireMockOpenApiParserDynamicExampleValues : IWireMockOpenApiParserExampleValues
    {
#pragma warning disable 1591
        public bool Boolean { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBoolean()).Generate() ?? true; } set { } }

        public int Integer { get { return RandomizerFactory.GetRandomizer(new FieldOptionsInteger()).Generate() ?? 42; } set { } }

        public float Float { get { return RandomizerFactory.GetRandomizer(new FieldOptionsFloat()).Generate() ?? 4.2f; } set { } }

        public double Double { get { return RandomizerFactory.GetRandomizer(new FieldOptionsDouble()).Generate() ?? 4.2d; } set { } }

        public Func<DateTime> Date { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow.Date; } set { } }

        public Func<DateTime> DateTime { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow; } set { } }

        public byte[] Bytes { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBytes()).Generate(); } set { } }

        public object Object { get; set; } = "example-object";

        public string String { get { return RandomizerFactory.GetRandomizer(new FieldOptionsTextWords()).Generate() ?? "example-string"; } set { } }
#pragma warning restore 1591
    }
}
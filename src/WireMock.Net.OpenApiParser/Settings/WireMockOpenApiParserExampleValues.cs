using System;

namespace WireMock.Net.OpenApiParser.Settings
{
    /// <summary>
    /// A class defining the example values to use for the different types.
    /// </summary>
    public class WireMockOpenApiParserExampleValues
    {
#pragma warning disable 1591
        public bool Boolean { get; set; } = true;

        public int Integer { get; set; } = 42;

        public float Float { get; set; } = 4.2f;

        public double Double { get; set; } = 4.2d;

        public Func<DateTime> Date { get; set; } = () => System.DateTime.UtcNow.Date;

        public Func<DateTime> DateTime { get; set; } = () => System.DateTime.UtcNow;

        public byte[] Bytes { get; set; } = { 48, 49, 50 };

        public object Object { get; set; } = "example-object";

        public string String { get; set; } = "example-string";
#pragma warning restore 1591
    }
}
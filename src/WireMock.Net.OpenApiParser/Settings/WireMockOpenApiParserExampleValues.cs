using System;
using Microsoft.OpenApi.Models;

namespace WireMock.Net.OpenApiParser.Settings
{
    /// <summary>
    /// A class defining the example values to use for the different types.
    /// </summary>
    public class WireMockOpenApiParserExampleValues : IWireMockOpenApiParserExampleValues
    {
        /// <inheritdoc />
        public bool Boolean { get; set; } = true;
        /// <inheritdoc />
        public int Integer { get; set; } = 42;
        /// <inheritdoc />
        public float Float { get; set; } = 4.2f;
        /// <inheritdoc />
        public double Double { get; set; } = 4.2d;
        /// <inheritdoc />
        public Func<DateTime> Date { get; set; } = () => System.DateTime.UtcNow.Date;
        /// <inheritdoc />
        public Func<DateTime> DateTime { get; set; } = () => System.DateTime.UtcNow;
        /// <inheritdoc />
        public byte[] Bytes { get; set; } = { 48, 49, 50 };
        /// <inheritdoc />
        public object Object { get; set; } = "example-object";
        /// <inheritdoc />
        public string String { get; set; } = "example-string";
        /// <inheritdoc />
        public OpenApiSchema Schema { get; set; } = new OpenApiSchema();
    }
}
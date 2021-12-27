using System;
using Microsoft.OpenApi.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using WireMock.Net.OpenApiParser.Settings;

namespace WireMock.Net.OpenApiParser.ConsoleApp
{

    public class DynamicDataGeneration : IWireMockOpenApiParserExampleValues
    {

        public bool Boolean { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBoolean()).Generate() ?? true; } set { } }

        public int Integer { get { return RandomizerFactory.GetRandomizer(new FieldOptionsInteger()).Generate() ?? 42; } set { } }

        public float Float { get { return RandomizerFactory.GetRandomizer(new FieldOptionsFloat()).Generate() ?? 4.2f; } set { } }

        public double Double { get { return RandomizerFactory.GetRandomizer(new FieldOptionsDouble()).Generate() ?? 4.2d; } set { } }

        public Func<DateTime> Date { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow.Date; } set { } }

        public Func<DateTime> DateTime { get { return () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow; } set { } }

        public byte[] Bytes { get { return RandomizerFactory.GetRandomizer(new FieldOptionsBytes()).Generate(); } set { } }

        public object Object { get; set; } = "example-object";

        public string String
        {
            get
            {
                //Since you have your Schema, you can get if max-lenght is set. You can generate accurate examples with this settings
                var maxLength = this.Schema.MaxLength ?? 9;

                return RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex
                {
                    Pattern = $"[0-9A-Z]{{{maxLength}}}"
                }).Generate() ?? "example-string";
            }
            set { }
        }

        public OpenApiSchema Schema { get; set; } = new OpenApiSchema();

    }

}

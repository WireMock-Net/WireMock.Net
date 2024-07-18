// Copyright © WireMock.Net

using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using WireMock.Net.OpenApiParser.Settings;

namespace WireMock.Net.OpenApiParser.ConsoleApp;

public class DynamicDataGeneration : WireMockOpenApiParserDynamicExampleValues
{
    public override string String
    {
        get
        {
            // Since you have your Schema, you can get if max-length is set. You can generate accurate examples with this settings
            var maxLength = Schema?.MaxLength ?? 9;

            return RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex
            {
                Pattern = $"[0-9A-Z]{{{maxLength}}}"
            }).Generate() ?? "example-string";
        }
    }
}
// Copyright © WireMock.Net

// Copied from https://github.com/Handlebars-Net/Handlebars.Net.Helpers/blob/master/src/Handlebars.Net.Helpers.DynamicLinq

namespace WireMock.Json;

internal class DynamicJsonClassOptions
{
    public IntegerBehavior IntegerConvertBehavior { get; set; } = IntegerBehavior.UseLong;

    public FloatBehavior FloatConvertBehavior { get; set; } = FloatBehavior.UseDouble;
}
// Copyright © WireMock.Net

// Copied from https://github.com/Handlebars-Net/Handlebars.Net.Helpers/blob/master/src/Handlebars.Net.Helpers.DynamicLinq

namespace WireMock.Json;

/// <summary>
/// Enum to define how to convert an Float in the Json Object.
/// </summary>
internal enum FloatBehavior
{
    /// <summary>
    /// Convert all Float types in the Json Object to a double. (default)
    /// </summary>
    UseDouble = 0,

    /// <summary>
    /// Convert all Float types in the Json Object to a float (unless overflow).
    /// </summary>
    UseFloat = 1,

    /// <summary>
    /// Convert all Float types in the Json Object to a decimal (unless overflow).
    /// </summary>
    UseDecimal = 2
}
// Copyright © WireMock.Net

// Copied from https://github.com/Handlebars-Net/Handlebars.Net.Helpers/blob/master/src/Handlebars.Net.Helpers.DynamicLinq

namespace WireMock.Json;

/// <summary>
/// Enum to define how to convert an Integer in the Json Object.
/// </summary>
internal enum IntegerBehavior
{
    /// <summary>
    /// Convert all Integer types in the Json Object to a int (unless overflow).
    /// (default)
    /// </summary>
    UseInt = 0,

    /// <summary>
    /// Convert all Integer types in the Json Object to a long.
    /// </summary>
    UseLong = 1
}
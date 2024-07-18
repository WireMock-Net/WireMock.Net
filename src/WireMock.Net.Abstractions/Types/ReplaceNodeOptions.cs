// Copyright © WireMock.Net

namespace WireMock.Types;

/// <summary>
/// Logic to use when replace a JSON node using the Transformer.
/// </summary>
public enum ReplaceNodeOptions
{
    /// <summary>
    /// Try to evaluate a templated value.
    /// In case this is valid, return the value and if the value can be converted to a supported (primitive) type, use that value.
    ///
    /// [Default behaviour]
    /// </summary>
    EvaluateAndTryToConvert = 0,

    /// <summary>
    /// Parse templated string to a templated string.
    /// For example: keep a templated string value (which is always the case) as a string value.
    /// </summary>
    Evaluate = 1,
    EvaluateAndKeep = Evaluate
}
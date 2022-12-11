namespace WireMock.Types;

/// <summary>
/// Logic to use when replace a JSON node using the Transformer.
/// </summary>
public enum ReplaceNodeOptions
{
    /// <summary>
    /// Try to evaluate a templated value.
    /// In case this is valid, return the value and if the value can be converted to a primitive type, use that value.
    /// </summary>
    EvaluateAndTryToConvert = 0,

    /// <summary>
    /// Try to evaluate a templated value.
    /// In case this is valid, return the value, else fallback to the parse behavior.
    /// </summary>
    Evaluate = 1,

    /// <summary>
    /// Parse templated string to a templated string.
    /// (keep a templated string value as string value).
    /// </summary>
    Parse = 2
}
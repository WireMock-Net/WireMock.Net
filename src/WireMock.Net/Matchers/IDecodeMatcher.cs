namespace WireMock.Matchers;

/// <summary>
/// IDecodeBytesMatcher
/// </summary>
public interface IDecodeBytesMatcher
{
    /// <summary>
    /// Decode byte array to an object.
    /// </summary>
    /// <param name="input">The byte array</param>
    /// <returns>object</returns>
    object? Decode(byte[]? input);
}
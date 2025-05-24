// Copyright © WireMock.Net

using System.Threading;
using System.Threading.Tasks;

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
    /// <param name="cancellationToken">The CancellationToken [optional].</param>
    /// <returns>object</returns>
    Task<object?> DecodeAsync(byte[]? input, CancellationToken cancellationToken = default);
}
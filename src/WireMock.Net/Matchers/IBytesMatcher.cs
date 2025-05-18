// Copyright © WireMock.Net

using System.Threading;
using System.Threading.Tasks;

namespace WireMock.Matchers;

/// <summary>
/// IBytesMatcher
/// </summary>
public interface IBytesMatcher : IMatcher
{
    /// <summary>
    /// Determines whether the specified input is match.
    /// </summary>
    /// <param name="input">The input byte array.</param>
    /// <param name="cancellationToken">The CancellationToken [optional].</param>
    /// <returns>MatchResult</returns>
    Task<MatchResult> IsMatchAsync(byte[]? input, CancellationToken cancellationToken = default);
}
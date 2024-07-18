// Copyright © WireMock.Net

namespace WireMock.Matchers;

/// <summary>
/// MatchBehaviour (Accept or Reject)
/// </summary>
public enum MatchBehaviour
{
    /// <summary>
    /// Accept on match (default)
    /// </summary>
    AcceptOnMatch,

    /// <summary>
    /// Reject on match
    /// </summary>
    RejectOnMatch
}
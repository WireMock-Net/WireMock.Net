// Copyright © WireMock.Net

namespace WireMock.Matchers.Request;

/// <summary>
/// The RequestMatcher interface.
/// </summary>
public interface IRequestMatcher
{
    /// <summary>
    /// Determines whether the specified RequestMessage is match.
    /// </summary>
    /// <param name="requestMessage">The RequestMessage.</param>
    /// <param name="requestMatchResult">The RequestMatchResult.</param>
    /// <returns>
    /// A value between 0.0 - 1.0 of the similarity.
    /// </returns>
    double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult);
}
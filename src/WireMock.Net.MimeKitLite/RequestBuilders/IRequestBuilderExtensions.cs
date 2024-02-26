// ReSharper disable InconsistentNaming
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilderExtensions extensions for MultiPart Mime using MimeKitLite.
/// </summary>
public static class IRequestBuilderExtensions
{
    /// <summary>
    /// WithMultiPart: IMatcher
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="matcher">The matcher.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithMultiPart(this IRequestBuilder requestBuilder, IMatcher matcher)
    {
        return requestBuilder.Add(new RequestMessageMultiPartMatcher(matcher));
    }

    /// <summary>
    /// WithMultiPart: IMatcher[], MatchBehaviour and MatchOperator
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchBehaviour">The <see cref="MatchBehaviour"/> to use.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithMultiPart(this IRequestBuilder requestBuilder, IMatcher[] matchers, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, MatchOperator matchOperator = MatchOperator.Or)
    {
        return requestBuilder.Add(new RequestMessageMultiPartMatcher(matchBehaviour, matchOperator, matchers));
    }

    /// <summary>
    /// WithMultiPart: MatchBehaviour and IMatcher[]
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="matchBehaviour">The <see cref="MatchBehaviour"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithMultiPart(this IRequestBuilder requestBuilder, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, params IMatcher[] matchers)
    {
        return requestBuilder.Add(new RequestMessageMultiPartMatcher(matchBehaviour, MatchOperator.Or, matchers));
    }
}
using System;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Util;

namespace WireMock.RequestBuilders;

/// <summary>
/// The BodyRequestBuilder interface.
/// </summary>
public interface IBodyRequestBuilder : IRequestMatcher
{
    /// <summary>
    /// WithBody: IMatcher
    /// </summary>
    /// <param name="matcher">The matcher.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(IMatcher matcher);

    /// <summary>
    /// WithBody: IMatcher[]
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(IMatcher[] matchers, MatchOperator matchOperator = MatchOperator.Or);

    /// <summary>
    /// WithBody: Body as string
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(string body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithBody: Body as byte[]
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(byte[] body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithBody: Body as object
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(object body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithBody: func (string)
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<string, bool> func);

    /// <summary>
    /// WithBody: func (byte[])
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<byte[], bool> func);

    /// <summary>
    /// WithBody: func (json object)
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<object, bool> func);

    /// <summary>
    /// WithBody: func (BodyData object)
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<IBodyData, bool> func);
}
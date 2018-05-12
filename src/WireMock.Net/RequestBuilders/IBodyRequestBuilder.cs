using System;
using JetBrains.Annotations;
using WireMock.Matchers;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The BodyRequestBuilder interface.
    /// </summary>
    public interface IBodyRequestBuilder
    {
        /// <summary>
        /// WithBody: IMatcher
        /// </summary>
        /// <param name="matcher">The matcher.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithBody([NotNull] IMatcher matcher);

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
        IRequestBuilder WithBody([NotNull] Func<string, bool> func);

        /// <summary>
        /// WithBody: func (byte[])
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithBody([NotNull] Func<byte[], bool> func);

        /// <summary>
        /// WithBody: func (object)
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithBody([NotNull] Func<object, bool> func);
    }
}
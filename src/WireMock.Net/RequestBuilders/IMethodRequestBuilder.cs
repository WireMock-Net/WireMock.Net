using System;
using JetBrains.Annotations;
using WireMock.Matchers;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The MethodRequestBuilder interface.
    /// </summary>
    public interface IMethodRequestBuilder : IHeadersAndCookiesRequestBuilder
    {
        /// <summary>
        /// UsingDelete: add HTTP Method matching on `delete` and matchBehaviour (optional).
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingDelete(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// UsingGet: add HTTP Method matching on `get` and matchBehaviour (optional).
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingGet(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// Add HTTP Method matching on `head` and matchBehaviour (optional).
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingHead(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// UsingPost: add HTTP Method matching on `post` and matchBehaviour (optional).
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingPost(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// UsingPatch: add HTTP Method matching on `patch` and matchBehaviour (optional).
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingPatch(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// UsingPut: add HTTP Method matching on `put` and matchBehaviour (optional).
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingPut(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

        /// <summary>
        /// UsingAnyMethod: add HTTP Method matching on any method.
        /// </summary>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingAnyMethod();

        /// <summary>
        /// UsingAnyVerb: add HTTP Method matching on any method.
        /// </summary>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        [Obsolete]
        IRequestBuilder UsingAnyVerb();

        /// <summary>
        /// UsingMethod: add HTTP Method matching on any methods and matchBehaviour.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="methods">The methods.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingMethod(MatchBehaviour matchBehaviour, [NotNull] params string[] methods);

        /// <summary>
        /// UsingMethod: add HTTP Method matching on any methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingMethod([NotNull] params string[] methods);

        /// <summary>
        /// UsingVerb: add HTTP Method matching on any methods.
        /// </summary>
        /// <param name="verbs">The methods.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        [Obsolete]
        IRequestBuilder UsingVerb([NotNull] params string[] verbs);
    }
}
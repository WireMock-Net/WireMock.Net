using JetBrains.Annotations;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The MethodRequestBuilder interface.
    /// </summary>
    public interface IMethodRequestBuilder : IHeadersAndCookiesRequestBuilder
    {
        /// <summary>
        /// The using get.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        IHeadersAndCookiesRequestBuilder UsingGet();

        /// <summary>
        /// The using post.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        IHeadersAndCookiesRequestBuilder UsingPost();

        /// <summary>
        /// The using delete.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        IHeadersAndCookiesRequestBuilder UsingDelete();

        /// <summary>
        /// The using put.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        IHeadersAndCookiesRequestBuilder UsingPut();

        /// <summary>
        /// The using head.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        IHeadersAndCookiesRequestBuilder UsingHead();

        /// <summary>
        /// The using any verb.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersAndCookiesRequestBuilder"/>.
        /// </returns>
        IHeadersAndCookiesRequestBuilder UsingAnyVerb();

        /// <summary>
        /// The using verb.
        /// </summary>
        /// <param name="verbs">The verb.</param>
        /// <returns>The <see cref="IHeadersAndCookiesRequestBuilder"/>.</returns>
        IHeadersAndCookiesRequestBuilder UsingVerb([NotNull] params string[] verbs);
    }
}
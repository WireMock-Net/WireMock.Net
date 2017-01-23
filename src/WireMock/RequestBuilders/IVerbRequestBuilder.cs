using JetBrains.Annotations;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The VerbRequestBuilder interface.
    /// </summary>
    public interface IVerbRequestBuilder : IHeadersRequestBuilder
    {
        /// <summary>
        /// The using get.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder UsingGet();

        /// <summary>
        /// The using post.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder UsingPost();

        /// <summary>
        /// The using delete.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder UsingDelete();

        /// <summary>
        /// The using put.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder UsingPut();

        /// <summary>
        /// The using head.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder UsingHead();

        /// <summary>
        /// The using any verb.
        /// </summary>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder UsingAnyVerb();

        /// <summary>
        /// The using verb.
        /// </summary>
        /// <param name="verbs">The verb.</param>
        /// <returns>The <see cref="IHeadersRequestBuilder"/>.</returns>
        IHeadersRequestBuilder UsingVerb([NotNull] params string[] verbs);
    }
}
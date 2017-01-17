namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The VerbRequestBuilder interface.
    /// </summary>
    public interface IVerbRequestBuilder : ISpecifyRequests, IHeadersRequestBuilder
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
        /// <param name="verb">
        /// The verb.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersRequestBuilder"/>.
        /// </returns>
        IHeadersRequestBuilder UsingVerb(string verb);
    }
}
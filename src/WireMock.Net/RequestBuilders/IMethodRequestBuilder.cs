using JetBrains.Annotations;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The MethodRequestBuilder interface.
    /// </summary>
    public interface IMethodRequestBuilder : IHeadersAndCookiesRequestBuilder
    {
        /// <summary>
        /// The using delete.
        /// </summary>
        /// <returns>
        /// The <see cref="IRequestBuilder"/>.
        /// </returns>
        IRequestBuilder UsingDelete();

        /// <summary>
        /// The using get.
        /// </summary>
        /// <returns>
        /// The <see cref="IRequestBuilder"/>.
        /// </returns>
        IRequestBuilder UsingGet();

        /// <summary>
        /// The using head.
        /// </summary>
        /// <returns>
        /// The <see cref="IRequestBuilder"/>.
        /// </returns>
        IRequestBuilder UsingHead();

        /// <summary>
        /// The using post.
        /// </summary>
        /// <returns>
        /// The <see cref="IRequestBuilder"/>.
        /// </returns>
        IRequestBuilder UsingPost();

        /// <summary>
        /// The using patch.
        /// </summary>
        /// <returns>
        /// The <see cref="IRequestBuilder"/>.
        /// </returns>
        IRequestBuilder UsingPatch();

        /// <summary>
        /// The using put.
        /// </summary>
        /// <returns>
        /// The <see cref="IRequestBuilder"/>.
        /// </returns>
        IRequestBuilder UsingPut();

        /// <summary>
        /// The using any verb.
        /// </summary>
        /// <returns>
        /// The <see cref="IRequestBuilder"/>.
        /// </returns>
        IRequestBuilder UsingAnyVerb();

        /// <summary>
        /// The using verb.
        /// </summary>
        /// <param name="verbs">The verb.</param>
        /// <returns>The <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder UsingVerb([NotNull] params string[] verbs);
    }
}
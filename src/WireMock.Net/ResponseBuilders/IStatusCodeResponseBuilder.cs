using System.Net;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The StatusCodeResponseBuilder interface.
    /// </summary>
    public interface IStatusCodeResponseBuilder : IHeadersResponseBuilder
    {
        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>The <see cref="IHeadersResponseBuilder"/>.</returns>
        IHeadersResponseBuilder WithStatusCode(int code);

        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>The <see cref="IHeadersResponseBuilder"/>.</returns>
        IHeadersResponseBuilder WithStatusCode(HttpStatusCode code);

        /// <summary>
        /// The with Success status code (200).
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IHeadersResponseBuilder WithSuccess();

        /// <summary>
        /// The with NotFound status code (404).
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IHeadersResponseBuilder WithNotFound();
    }
}
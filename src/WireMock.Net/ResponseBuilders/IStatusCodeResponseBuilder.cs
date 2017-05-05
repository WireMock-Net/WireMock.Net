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
        /// <param name="code">The code.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithStatusCode(int code);

        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithStatusCode(HttpStatusCode code);

        /// <summary>
        /// The with Success status code (200).
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithSuccess();

        /// <summary>
        /// The with NotFound status code (404).
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithNotFound();
    }
}
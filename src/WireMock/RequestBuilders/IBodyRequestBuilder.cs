using System;

namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The BodyRequestBuilder interface.
    /// </summary>
    public interface IBodyRequestBuilder
    {
        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithBody(string body);
        
        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">
        /// The body function.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithBody(Func<string, bool> body);
    }
}
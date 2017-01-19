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
        /// The with body.
        /// </summary>
        /// <param name="matcher">
        /// The matcher.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithBody([NotNull] IMatcher matcher);

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
        /// The with body byte[].
        /// </summary>
        /// <param name="body">
        /// The body as byte[].
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithBody(byte[] body);

        /// <summary>
        /// The with body string func.
        /// </summary>
        /// <param name="body">
        /// The body string function.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithBody(Func<string, bool> body);

        /// <summary>
        /// The with body byte[] func.
        /// </summary>
        /// <param name="body">
        /// The body byte[] function.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithBody(Func<byte[], bool> body);
    }
}
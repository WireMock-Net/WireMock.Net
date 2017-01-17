using System;
using System.Diagnostics.CodeAnalysis;

[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]

namespace WireMock
{
    /// <summary>
    /// The HeadersResponseBuilder interface.
    /// </summary>
    public interface IHeadersResponseBuilder : IBodyResponseBuilder
    {
        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IHeadersResponseBuilder"/>.
        /// </returns>
        IHeadersResponseBuilder WithHeader(string name, string value);
    }

    /// <summary>
    /// The BodyResponseBuilder interface.
    /// </summary>
    public interface IBodyResponseBuilder : IDelayResponseBuilder
    {
        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The <see cref="IDelayResponseBuilder"/>.
        /// </returns>
        IDelayResponseBuilder WithBody(string body);
    }

    /// <summary>
    /// The DelayResponseBuilder interface.
    /// </summary>
    public interface IDelayResponseBuilder : IProvideResponses
    {
        /// <summary>
        /// The after delay.
        /// </summary>
        /// <param name="delay">
        /// The delay.
        /// </param>
        /// <returns>
        /// The <see cref="IProvideResponses"/>.
        /// </returns>
        IProvideResponses AfterDelay(TimeSpan delay);
    }
}

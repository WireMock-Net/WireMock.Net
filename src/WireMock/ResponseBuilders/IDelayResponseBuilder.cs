using System;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The DelayResponseBuilder interface.
    /// </summary>
    public interface IDelayResponseBuilder : IResponseProvider
    {
        /// <summary>
        /// The after delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        IResponseBuilder WithDelay(TimeSpan delay);
    }
}
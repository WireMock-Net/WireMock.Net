using System;

namespace WireMock.ResponseBuilders
{
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
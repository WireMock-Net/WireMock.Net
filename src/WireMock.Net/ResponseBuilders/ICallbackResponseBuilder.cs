using System;
using JetBrains.Annotations;
using WireMock.ResponseProviders;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The CallbackResponseBuilder interface.
    /// </summary>
    public interface ICallbackResponseBuilder : IResponseProvider
    {
        /// <summary>
        /// The callback builder
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        IResponseBuilder WithCallback([NotNull] Func<RequestMessage, ResponseMessage> callbackHandler);
    }
}
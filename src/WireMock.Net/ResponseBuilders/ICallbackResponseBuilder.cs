using System;

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
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        IResponseBuilder WithCallback(Func<RequestMessage,ResponseMessage> callbackHandler);
    }
}
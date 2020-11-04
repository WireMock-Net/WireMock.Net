using System;
using System.Threading.Tasks;
using WireMock.Validation;

namespace WireMock.ResponseBuilders
{
    public partial class Response
    {
        /// <summary>
        /// A delegate to execute to generate the response.
        /// </summary>
        public Func<RequestMessage, ResponseMessage> Callback { get; private set; }

        /// <summary>
        /// A delegate to execute to generate the response async.
        /// </summary>
        public Func<RequestMessage, Task<ResponseMessage>> CallbackAsync { get; private set; }

        /// <summary>
        /// Defines if the method WithCallback(...) is used.
        /// </summary>
        public bool WithCallbackUsed { get; private set; }

        /// <inheritdoc cref="ICallbackResponseBuilder.WithCallback(Func{RequestMessage, ResponseMessage})"/>
        public IResponseBuilder WithCallback(Func<RequestMessage, ResponseMessage> callbackHandler)
        {
            Check.NotNull(callbackHandler, nameof(callbackHandler));

            return WithCallbackInternal(true, callbackHandler);
        }

        /// <inheritdoc cref="ICallbackResponseBuilder.WithCallback(Func{RequestMessage, Task{ResponseMessage}})"/>
        public IResponseBuilder WithCallback(Func<RequestMessage, Task<ResponseMessage>> callbackHandler)
        {
            Check.NotNull(callbackHandler, nameof(callbackHandler));

            return WithCallbackInternal(true, callbackHandler);
        }

        private IResponseBuilder WithCallbackInternal(bool withCallbackUsed, Func<RequestMessage, ResponseMessage> callbackHandler)
        {
            Check.NotNull(callbackHandler, nameof(callbackHandler));

            WithCallbackUsed = withCallbackUsed;
            Callback = callbackHandler;

            return this;
        }

        private IResponseBuilder WithCallbackInternal(bool withCallbackUsed, Func<RequestMessage, Task<ResponseMessage>> callbackHandler)
        {
            Check.NotNull(callbackHandler, nameof(callbackHandler));

            WithCallbackUsed = withCallbackUsed;
            CallbackAsync = callbackHandler;

            return this;
        }
    }
}
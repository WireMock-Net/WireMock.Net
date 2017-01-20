using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using JetBrains.Annotations;
using WireMock.Validation;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The Response.
    /// </summary>
    public class Response : IResponseBuilder
    {
        private readonly ResponseMessage _responseMessage;
        private TimeSpan _delay = TimeSpan.Zero;
        private bool _useTransformer;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public static IResponseBuilder Create([CanBeNull] ResponseMessage responseMessage = null)
        {
            var message = responseMessage ?? new ResponseMessage { StatusCode = (int) HttpStatusCode.OK };
            return new Response(message);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="responseMessage">
        /// The response.
        /// </param>
        private Response(ResponseMessage responseMessage)
        {
            _responseMessage = responseMessage;
        }

        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A <see cref="IHeadersResponseBuilder"/>.</returns>\
        [PublicAPI]
        public IHeadersResponseBuilder WithStatusCode(int code)
        {
            _responseMessage.StatusCode = code;
            return this;
        }

        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A <see cref="IHeadersResponseBuilder"/>.</returns>
        [PublicAPI]
        public IHeadersResponseBuilder WithStatusCode(HttpStatusCode code)
        {
            return WithStatusCode((int) code);
        }

        /// <summary>
        /// The with Success status code (200).
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public IHeadersResponseBuilder WithSuccess()
        {
            return WithStatusCode((int) HttpStatusCode.OK);
        }

        /// <summary>
        /// The with NotFound status code (404).
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public IHeadersResponseBuilder WithNotFound()
        {
            return WithStatusCode((int)HttpStatusCode.NotFound);
        }

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="IHeadersResponseBuilder"/>.</returns>
        public IHeadersResponseBuilder WithHeader(string name, string value)
        {
            Check.NotNull(name, nameof(name));

            _responseMessage.AddHeader(name, value);
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>A <see cref="ITransformResponseBuilder"/>.</returns>
        public ITransformResponseBuilder WithBody(string body)
        {
            Check.NotNull(body, nameof(body));

            _responseMessage.Body = body;
            return this;
        }

        /// <summary>
        /// The with body as base64.
        /// </summary>
        /// <param name="bodyAsbase64">The body asbase64.</param>
        /// <param name="encoding">The Encoding.</param>
        /// <returns>A <see cref="ITransformResponseBuilder"/>.</returns>
        public ITransformResponseBuilder WithBodyAsBase64(string bodyAsbase64, Encoding encoding = null)
        {
            Check.NotNull(bodyAsbase64, nameof(bodyAsbase64));

            _responseMessage.Body = (encoding ?? Encoding.UTF8).GetString(Convert.FromBase64String(bodyAsbase64));
            return this;
        }

        /// <summary>
        /// The with transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        public IDelayResponseBuilder WithTransformer()
        {
            _useTransformer = true;
            return this;
        }

        /// <summary>
        /// The after delay.
        /// </summary>
        /// <param name="delay">
        /// The delay.
        /// </param>
        /// <returns>
        /// The <see cref="IProvideResponses"/>.
        /// </returns>
        public IResponseBuilder WithDelay(TimeSpan delay)
        {
            _delay = delay;
            return this;
        }

        /// <summary>
        /// The provide response.
        /// </summary>
        /// <param name="requestMessage">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ResponseMessage> ProvideResponse(RequestMessage requestMessage)
        {
            if (_useTransformer)
            {
                var template = new { request = requestMessage };

                // Body
                var templateBody = Handlebars.Compile(_responseMessage.Body);
                _responseMessage.Body = templateBody(template);

                // Headers
                var newHeaders = new Dictionary<string, string>();
                foreach (var header in _responseMessage.Headers)
                {
                    var templateHeaderKey = Handlebars.Compile(header.Key);
                    var templateHeaderValue = Handlebars.Compile(header.Value);

                    newHeaders.Add(templateHeaderKey(template), templateHeaderValue(template));
                }
                _responseMessage.Headers = newHeaders;
            }

            await Task.Delay(_delay);

            return _responseMessage;
        }
    }
}
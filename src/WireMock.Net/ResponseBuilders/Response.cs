using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Validation;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The Response.
    /// </summary>
    public class Response : IResponseBuilder
    {
        private TimeSpan _delay = TimeSpan.Zero;

        /// <summary>
        /// Gets a value indicating whether [use transformer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use transformer]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTransformer { get; private set; }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        public ResponseMessage ResponseMessage { get; }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="responseMessage">ResponseMessage</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public static IResponseBuilder Create([CanBeNull] ResponseMessage responseMessage = null)
        {
            var message = responseMessage ?? new ResponseMessage { StatusCode = (int)HttpStatusCode.OK };
            return new Response(message);
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public static IResponseBuilder Create([NotNull] Func<ResponseMessage> func)
        {
            Check.NotNull(func, nameof(func));

            return new Response(func());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="responseMessage">
        /// The response.
        /// </param>
        private Response(ResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }

        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>\
        [PublicAPI]
        public IResponseBuilder WithStatusCode(int code)
        {
            ResponseMessage.StatusCode = code;
            return this;
        }

        /// <summary>
        /// The with status code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public IResponseBuilder WithStatusCode(HttpStatusCode code)
        {
            return WithStatusCode((int)code);
        }

        /// <summary>
        /// The with Success status code (200).
        /// </summary>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public IResponseBuilder WithSuccess()
        {
            return WithStatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// The with NotFound status code (404).
        /// </summary>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public IResponseBuilder WithNotFound()
        {
            return WithStatusCode((int)HttpStatusCode.NotFound);
        }

        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithHeader(string name, string value)
        {
            Check.NotNull(name, nameof(name));

            ResponseMessage.AddHeader(name, value);
            return this;
        }

        /// <summary>
        /// The with headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public IResponseBuilder WithHeaders(IDictionary<string, string> headers)
        {
            ResponseMessage.Headers = headers;
            return this;
        }

        /// <summary>
        /// The with body.
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithBody(string body)
        {
            Check.NotNull(body, nameof(body));

            ResponseMessage.Body = body;
            return this;
        }

        /// <summary>
        /// The with body (AsJson object).
        /// </summary>
        /// <param name="body">The body.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithBodyAsJson(object body)
        {
            Check.NotNull(body, nameof(body));

            ResponseMessage.Body = JsonConvert.SerializeObject(body, new JsonSerializerSettings { Formatting =  Formatting.None, NullValueHandling = NullValueHandling.Ignore } );
            return this;
        }

        /// <summary>
        /// The with body as base64.
        /// </summary>
        /// <param name="bodyAsbase64">The body asbase64.</param>
        /// <param name="encoding">The Encoding.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithBodyAsBase64(string bodyAsbase64, Encoding encoding = null)
        {
            Check.NotNull(bodyAsbase64, nameof(bodyAsbase64));

            ResponseMessage.Body = (encoding ?? Encoding.UTF8).GetString(Convert.FromBase64String(bodyAsbase64));
            return this;
        }

        /// <summary>
        /// The with transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        public IResponseBuilder WithTransformer()
        {
            UseTransformer = true;
            return this;
        }

        /// <summary>
        /// The after delay.
        /// </summary>
        /// <param name="delay">
        /// The delay.
        /// </param>
        /// <returns>
        /// The <see cref="IResponseProvider"/>.
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
            ResponseMessage responseMessage;
            if (UseTransformer)
            {
                responseMessage = new ResponseMessage { StatusCode = ResponseMessage.StatusCode, BodyOriginal = ResponseMessage.Body };

                var template = new { request = requestMessage };

                // Body
                var templateBody = Handlebars.Compile(ResponseMessage.Body);
                responseMessage.Body = templateBody(template);

                // Headers
                var newHeaders = new Dictionary<string, string>();
                foreach (var header in ResponseMessage.Headers)
                {
                    var templateHeaderKey = Handlebars.Compile(header.Key);
                    var templateHeaderValue = Handlebars.Compile(header.Value);

                    newHeaders.Add(templateHeaderKey(template), templateHeaderValue(template));
                }
                responseMessage.Headers = newHeaders;
            }
            else
            {
                responseMessage = ResponseMessage;
            }

            await Task.Delay(_delay);

            return responseMessage;
        }
    }
}
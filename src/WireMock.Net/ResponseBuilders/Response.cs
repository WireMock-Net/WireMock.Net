using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Validation;
using WireMock.Http;
using WireMock.Transformers;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The Response.
    /// </summary>
    public class Response : IResponseBuilder
    {
        /// <summary>
        /// The delay
        /// </summary>
        public TimeSpan? Delay { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use transformer].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use transformer]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTransformer { get; private set; }

        /// <summary>
        /// The Proxy URL to use.
        /// </summary>
        public string ProxyUrl { get; private set; }

        /// <summary>
        /// The client X509Certificate2 Thumbprint or SubjectName to use.
        /// </summary>
        public string X509Certificate2ThumbprintOrSubjectName { get; private set; }

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

        /// <inheritdoc cref="IBodyResponseBuilder.WithBody(byte[], string, Encoding)"/>
        public IResponseBuilder WithBody(byte[] body, string destination, Encoding encoding = null)
        {
            Check.NotNull(body, nameof(body));

            ResponseMessage.BodyDestination = destination;

            switch (destination)
            {
                case BodyDestinationFormat.String:
                    var enc = encoding ?? Encoding.UTF8;
                    ResponseMessage.BodyAsBytes = null;
                    ResponseMessage.Body = enc.GetString(body);
                    ResponseMessage.BodyEncoding = enc;
                    break;

                default:
                    ResponseMessage.BodyAsBytes = body;
                    ResponseMessage.BodyEncoding = null;
                    break;
            }

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBody(string, string, Encoding)"/>
        public IResponseBuilder WithBody(string body, string destination = BodyDestinationFormat.SameAsSource, Encoding encoding = null)
        {
            Check.NotNull(body, nameof(body));

            encoding = encoding ?? Encoding.UTF8;

            ResponseMessage.BodyDestination = destination;

            switch (destination)
            {
                case BodyDestinationFormat.Bytes:
                    ResponseMessage.Body = null;
                    ResponseMessage.BodyAsBytes = encoding.GetBytes(body);
                    ResponseMessage.BodyEncoding = encoding;
                    break;

                default:
                    ResponseMessage.Body = body;
                    ResponseMessage.BodyAsBytes = null;
                    ResponseMessage.BodyEncoding = encoding;
                    break;
            }

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyAsJson"/>
        public IResponseBuilder WithBodyAsJson(object body, Encoding encoding = null)
        {
            Check.NotNull(body, nameof(body));

            string jsonBody = JsonConvert.SerializeObject(body, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore });

            if (encoding != null && !encoding.Equals(Encoding.UTF8))
            {
                jsonBody = encoding.GetString(Encoding.UTF8.GetBytes(jsonBody));
                ResponseMessage.BodyEncoding = encoding;
            }

            ResponseMessage.BodyDestination = null;
            ResponseMessage.Body = jsonBody;

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyFromBase64"/>
        public IResponseBuilder WithBodyFromBase64(string bodyAsbase64, Encoding encoding = null)
        {
            Check.NotNull(bodyAsbase64, nameof(bodyAsbase64));

            encoding = encoding ?? Encoding.UTF8;

            ResponseMessage.BodyDestination = null;
            ResponseMessage.Body = encoding.GetString(Convert.FromBase64String(bodyAsbase64));
            ResponseMessage.BodyEncoding = encoding;

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
        /// The with delay.
        /// </summary>
        /// <param name="delay">The TimeSpan to delay.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithDelay(TimeSpan delay)
        {
            Check.Condition(delay, d => d > TimeSpan.Zero, nameof(delay));
            Delay = delay;
            return this;
        }

        /// <summary>
        /// The with delay.
        /// </summary>
        /// <param name="milliseconds">The milliseconds to delay.</param>
        /// <returns>The <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithDelay(int milliseconds)
        {
            return WithDelay(TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// With Proxy URL.
        /// </summary>
        /// <param name="proxyUrl">The proxy url.</param>
        /// <param name="clientX509Certificate2ThumbprintOrSubjectName">The X509Certificate2 file to use for client authentication.</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        public IResponseBuilder WithProxy(string proxyUrl, string clientX509Certificate2ThumbprintOrSubjectName = null)
        {
            Check.NotEmpty(proxyUrl, nameof(proxyUrl));

            ProxyUrl = proxyUrl;
            X509Certificate2ThumbprintOrSubjectName = clientX509Certificate2ThumbprintOrSubjectName;
            return this;
        }

        /// <summary>
        /// The provide response.
        /// </summary>
        /// <param name="requestMessage">The request.</param>
        /// <returns>The <see cref="ResponseMessage"/>.</returns>
        public async Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage)
        {
            Check.NotNull(requestMessage, nameof(requestMessage));

            if (Delay != null)
                await Task.Delay(Delay.Value);

            if (ProxyUrl != null)
            {
                var requestUri = new Uri(requestMessage.Url);
                var proxyUri = new Uri(ProxyUrl);
                var proxyUriWithRequestPathAndQuery = new Uri(proxyUri, requestUri.PathAndQuery);
                return await HttpClientHelper.SendAsync(requestMessage, proxyUriWithRequestPathAndQuery.AbsoluteUri, X509Certificate2ThumbprintOrSubjectName);
            }

            if (UseTransformer)
            {
                return ResponseMessageTransformer.Transform(requestMessage, ResponseMessage);
            }

            return ResponseMessage;
        }
    }
}
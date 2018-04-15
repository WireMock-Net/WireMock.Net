using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Http;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The Response.
    /// </summary>
    public class Response : IResponseBuilder
    {
        private HttpClient _httpClientForProxy;

        /// <summary>
        /// The delay
        /// </summary>
        public TimeSpan? Delay { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use transformer].
        /// </summary>
        public bool UseTransformer { get; private set; }

        /// <summary>
        /// The Proxy URL to use.
        /// </summary>
        public string ProxyUrl { get; private set; }

        /// <summary>
        /// The client X509Certificate2 Thumbprint or SubjectName to use.
        /// </summary>
        public string ClientX509Certificate2ThumbprintOrSubjectName { get; private set; }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public ResponseMessage ResponseMessage { get; }

        /// <summary>
        /// A delegate to execute to generate the response
        /// </summary>
        public Func<RequestMessage, ResponseMessage> Callback { get; private set; }

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
        /// Creates this instance with the specified function.
        /// </summary>
        /// <param name="func">The callback function.</param>
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

        /// <inheritdoc cref="IHeadersResponseBuilder.WithHeader(string, string[])"/>
        public IResponseBuilder WithHeader(string name, params string[] values)
        {
            Check.NotNull(name, nameof(name));

            ResponseMessage.AddHeader(name, values);
            return this;
        }

        /// <inheritdoc cref="IHeadersResponseBuilder.WithHeaders(IDictionary{string, string})"/>
        public IResponseBuilder WithHeaders(IDictionary<string, string> headers)
        {
            Check.NotNull(headers, nameof(headers));

            ResponseMessage.Headers = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
            return this;
        }

        /// <inheritdoc cref="IHeadersResponseBuilder.WithHeaders(IDictionary{string, string[]})"/>
        public IResponseBuilder WithHeaders(IDictionary<string, string[]> headers)
        {
            Check.NotNull(headers, nameof(headers));

            ResponseMessage.Headers = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
            return this;
        }

        /// <inheritdoc cref="IHeadersResponseBuilder.WithHeaders(IDictionary{string, WireMockList{string}})"/>
        public IResponseBuilder WithHeaders(IDictionary<string, WireMockList<string>> headers)
        {
            ResponseMessage.Headers = headers;
            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBody(Func{RequestMessage, string}, string, Encoding)"/>
        public IResponseBuilder WithBody(Func<RequestMessage, string> bodyFactory, string destination = BodyDestinationFormat.SameAsSource, Encoding encoding = null)
        {
            return WithCallback(req => new ResponseMessage { Body = bodyFactory(req) });
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

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyFromFile"/>
        public IResponseBuilder WithBodyFromFile(string filename, bool cache = true)
        {
            Check.NotNull(filename, nameof(filename));

            ResponseMessage.BodyEncoding = null;
            ResponseMessage.BodyAsFileIsCached = cache;

            if (cache)
            {
                ResponseMessage.Body = null;
                ResponseMessage.BodyAsBytes = File.ReadAllBytes(filename);
                ResponseMessage.BodyAsFile = null;
            }
            else
            {
                ResponseMessage.Body = null;
                ResponseMessage.BodyAsBytes = null;
                ResponseMessage.BodyAsFile = filename;
            }

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBody(string, string, Encoding)"/>
        public IResponseBuilder WithBody(string body, string destination = BodyDestinationFormat.SameAsSource, Encoding encoding = null)
        {
            Check.NotNull(body, nameof(body));

            encoding = encoding ?? Encoding.UTF8;

            ResponseMessage.BodyDestination = destination;
            ResponseMessage.BodyEncoding = encoding;

            switch (destination)
            {
                case BodyDestinationFormat.Bytes:
                    ResponseMessage.Body = null;
                    ResponseMessage.BodyAsJson = null;
                    ResponseMessage.BodyAsBytes = encoding.GetBytes(body);
                    break;

                case BodyDestinationFormat.Json:
                    ResponseMessage.Body = null;
                    ResponseMessage.BodyAsJson = JsonConvert.DeserializeObject(body);
                    ResponseMessage.BodyAsBytes = null;
                    break;

                default:
                    ResponseMessage.Body = body;
                    ResponseMessage.BodyAsJson = null;
                    ResponseMessage.BodyAsBytes = null;
                    break;
            }

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyAsJson(object, Encoding, bool?)"/>
        public IResponseBuilder WithBodyAsJson(object body, Encoding encoding = null, bool? indented = null)
        {
            Check.NotNull(body, nameof(body));

            ResponseMessage.BodyDestination = null;
            ResponseMessage.BodyAsJson = body;
            ResponseMessage.BodyEncoding = encoding;
            ResponseMessage.BodyAsJsonIndented = indented;

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyAsJson(object, bool)"/>
        public IResponseBuilder WithBodyAsJson(object body, bool indented)
        {
            return WithBodyAsJson(body, null, indented);
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

        /// <inheritdoc cref="ITransformResponseBuilder.WithTransformer"/>
        public IResponseBuilder WithTransformer()
        {
            UseTransformer = true;
            return this;
        }

        /// <inheritdoc cref="IDelayResponseBuilder.WithDelay(TimeSpan)"/>
        public IResponseBuilder WithDelay(TimeSpan delay)
        {
            Check.Condition(delay, d => d > TimeSpan.Zero, nameof(delay));

            Delay = delay;
            return this;
        }

        /// <inheritdoc cref="IDelayResponseBuilder.WithDelay(int)"/>
        public IResponseBuilder WithDelay(int milliseconds)
        {
            return WithDelay(TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <inheritdoc cref="IProxyResponseBuilder.WithProxy(string, string)"/>
        public IResponseBuilder WithProxy(string proxyUrl, string clientX509Certificate2ThumbprintOrSubjectName = null)
        {
            Check.NotNullOrEmpty(proxyUrl, nameof(proxyUrl));

            ProxyUrl = proxyUrl;
            ClientX509Certificate2ThumbprintOrSubjectName = clientX509Certificate2ThumbprintOrSubjectName;
            _httpClientForProxy = HttpClientHelper.CreateHttpClient(clientX509Certificate2ThumbprintOrSubjectName);
            return this;
        }

        /// <inheritdoc cref="IProxyResponseBuilder.WithProxy(IProxyAndRecordSettings)"/>
        public IResponseBuilder WithProxy(IProxyAndRecordSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            return WithProxy(settings.Url, settings.ClientX509Certificate2ThumbprintOrSubjectName);
        }

        /// <inheritdoc cref="ICallbackResponseBuilder.WithCallback"/>
        public IResponseBuilder WithCallback(Func<RequestMessage, ResponseMessage> callbackHandler)
        {
            Check.NotNull(callbackHandler, nameof(callbackHandler));

            Callback = callbackHandler;

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
            {
                await Task.Delay(Delay.Value);
            }

            if (ProxyUrl != null && _httpClientForProxy != null)
            {
                var requestUri = new Uri(requestMessage.Url);
                var proxyUri = new Uri(ProxyUrl);
                var proxyUriWithRequestPathAndQuery = new Uri(proxyUri, requestUri.PathAndQuery);

                return await HttpClientHelper.SendAsync(_httpClientForProxy, requestMessage, proxyUriWithRequestPathAndQuery.AbsoluteUri);
            }

            if (UseTransformer)
            {
                return ResponseMessageTransformer.Transform(requestMessage, ResponseMessage);
            }

            if (Callback != null)
            {
                return Callback(requestMessage);
            }

            return ResponseMessage;
        }
    }
}
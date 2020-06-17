// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.Http;
using WireMock.ResponseProviders;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Types;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The Response.
    /// </summary>
    public partial class Response : IResponseBuilder
    {
        /// <summary>
        /// The delay
        /// </summary>
        public TimeSpan? Delay { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use transformer].
        /// </summary>
        public bool UseTransformer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to use the Handlerbars transformer for the content from the referenced BodyAsFile.
        /// </summary>
        public bool UseTransformerForBodyAsFile { get; private set; }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public ResponseMessage ResponseMessage { get; }

        /// <summary>
        /// A delegate to execute to generate the response.
        /// </summary>
        public Func<RequestMessage, ResponseMessage> Callback { get; private set; }

        /// <summary>
        /// Defines if the method WithCallback(...) is used.
        /// </summary>
        public bool WithCallbackUsed { get; private set; }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="responseMessage">ResponseMessage</param>
        /// <returns>A <see cref="IResponseBuilder"/>.</returns>
        [PublicAPI]
        public static IResponseBuilder Create([CanBeNull] ResponseMessage responseMessage = null)
        {
            var message = responseMessage ?? new ResponseMessage(); // { StatusCode = (int)HttpStatusCode.OK };
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

        /// <inheritdoc cref="IStatusCodeResponseBuilder.WithStatusCode(int)"/>
        [PublicAPI]
        public IResponseBuilder WithStatusCode(int code)
        {
            ResponseMessage.StatusCode = code;
            return this;
        }

        /// <inheritdoc cref="IStatusCodeResponseBuilder.WithStatusCode(string)"/>
        [PublicAPI]
        public IResponseBuilder WithStatusCode(string code)
        {
            ResponseMessage.StatusCode = code;
            return this;
        }

        /// <inheritdoc cref="IStatusCodeResponseBuilder.WithStatusCode(HttpStatusCode)"/>
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
            Check.NotNull(bodyFactory, nameof(bodyFactory));

            return WithCallbackInternal(false, req => new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = bodyFactory(req),
                    Encoding = encoding ?? Encoding.UTF8
                }
            });
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBody(byte[], string, Encoding)"/>
        public IResponseBuilder WithBody(byte[] body, string destination = BodyDestinationFormat.SameAsSource, Encoding encoding = null)
        {
            Check.NotNull(body, nameof(body));

            ResponseMessage.BodyDestination = destination;
            ResponseMessage.BodyData = new BodyData();

            switch (destination)
            {
                case BodyDestinationFormat.String:
                    var enc = encoding ?? Encoding.UTF8;
                    ResponseMessage.BodyData.DetectedBodyType = BodyType.String;
                    ResponseMessage.BodyData.BodyAsString = enc.GetString(body);
                    ResponseMessage.BodyData.Encoding = enc;
                    break;

                default:
                    ResponseMessage.BodyData.DetectedBodyType = BodyType.Bytes;
                    ResponseMessage.BodyData.BodyAsBytes = body;
                    break;
            }

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyFromFile"/>
        public IResponseBuilder WithBodyFromFile(string filename, bool cache = true)
        {
            Check.NotNull(filename, nameof(filename));

            ResponseMessage.BodyData = new BodyData
            {
                BodyAsFileIsCached = cache,
                BodyAsFile = filename
            };

            if (cache && !UseTransformer)
            {
                ResponseMessage.BodyData.DetectedBodyType = BodyType.Bytes;
            }
            else
            {
                ResponseMessage.BodyData.DetectedBodyType = BodyType.File;
            }

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBody(string, string, Encoding)"/>
        public IResponseBuilder WithBody(string body, string destination = BodyDestinationFormat.SameAsSource, Encoding encoding = null)
        {
            Check.NotNull(body, nameof(body));

            encoding = encoding ?? Encoding.UTF8;

            ResponseMessage.BodyDestination = destination;

            ResponseMessage.BodyData = new BodyData
            {
                Encoding = encoding
            };

            switch (destination)
            {
                case BodyDestinationFormat.Bytes:
                    ResponseMessage.BodyData.DetectedBodyType = BodyType.Bytes;
                    ResponseMessage.BodyData.BodyAsBytes = encoding.GetBytes(body);
                    break;

                case BodyDestinationFormat.Json:
                    ResponseMessage.BodyData.DetectedBodyType = BodyType.Json;
                    ResponseMessage.BodyData.BodyAsJson = JsonUtils.DeserializeObject(body);
                    break;

                default:
                    ResponseMessage.BodyData.DetectedBodyType = BodyType.String;
                    ResponseMessage.BodyData.BodyAsString = body;
                    break;
            }

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyAsJson(object, Encoding, bool?)"/>
        public IResponseBuilder WithBodyAsJson(object body, Encoding encoding = null, bool? indented = null)
        {
            Check.NotNull(body, nameof(body));

            ResponseMessage.BodyDestination = null;
            ResponseMessage.BodyData = new BodyData
            {
                Encoding = encoding,
                DetectedBodyType = BodyType.Json,
                BodyAsJson = body,
                BodyAsJsonIndented = indented
            };

            return this;
        }

        /// <inheritdoc cref="IBodyResponseBuilder.WithBodyAsJson(object, bool)"/>
        public IResponseBuilder WithBodyAsJson(object body, bool indented)
        {
            return WithBodyAsJson(body, null, indented);
        }

        /// <inheritdoc cref="ITransformResponseBuilder.WithTransformer(bool)"/>
        public IResponseBuilder WithTransformer(bool transformContentFromBodyAsFile = false)
        {
            UseTransformer = true;
            UseTransformerForBodyAsFile = transformContentFromBodyAsFile;
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

        /// <inheritdoc cref="ICallbackResponseBuilder.WithCallback"/>
        public IResponseBuilder WithCallback(Func<RequestMessage, ResponseMessage> callbackHandler)
        {
            Check.NotNull(callbackHandler, nameof(callbackHandler));

            return WithCallbackInternal(true, callbackHandler);
        }

        /// <inheritdoc cref="ICallbackResponseBuilder.WithCallback"/>
        private IResponseBuilder WithCallbackInternal(bool withCallbackUsed, Func<RequestMessage, ResponseMessage> callbackHandler)
        {
            Check.NotNull(callbackHandler, nameof(callbackHandler));

            WithCallbackUsed = withCallbackUsed;
            Callback = callbackHandler;

            return this;
        }

        /// <inheritdoc cref="IResponseProvider.ProvideResponseAsync(RequestMessage, IWireMockServerSettings)"/>
        public async Task<ResponseMessage> ProvideResponseAsync(RequestMessage requestMessage, IWireMockServerSettings settings)
        {
            Check.NotNull(requestMessage, nameof(requestMessage));
            Check.NotNull(settings, nameof(settings));

            if (Delay != null)
            {
                await Task.Delay(Delay.Value);
            }

            if (ProxyUrl != null && _httpClientForProxy != null)
            {
                string RemoveFirstOccurrence(string source, string find)
                {
                    int place = source.IndexOf(find, StringComparison.OrdinalIgnoreCase);
                    return place >= 0 ? source.Remove(place, find.Length) : source;
                }

                var requestUri = new Uri(requestMessage.Url);

                // Build the proxy url and skip duplicates
                string extra = RemoveFirstOccurrence(requestUri.LocalPath.TrimEnd('/'), new Uri(ProxyUrl).LocalPath.TrimEnd('/'));
                requestMessage.ProxyUrl = ProxyUrl + extra + requestUri.Query;

                return await HttpClientHelper.SendAsync(
                    _httpClientForProxy,
                    requestMessage,
                    requestMessage.ProxyUrl,
                    !settings.DisableJsonBodyParsing.GetValueOrDefault(false),
                    !settings.DisableRequestBodyDecompressing.GetValueOrDefault(false)
                );
            }

            ResponseMessage responseMessage;
            if (Callback == null)
            {
                responseMessage = ResponseMessage;
            }
            else
            {
                responseMessage = Callback(requestMessage);

                if (!WithCallbackUsed)
                {
                    // Copy StatusCode from ResponseMessage
                    responseMessage.StatusCode = ResponseMessage.StatusCode;

                    // Copy Headers from ResponseMessage (if defined)
                    if (ResponseMessage.Headers != null)
                    {
                        responseMessage.Headers = ResponseMessage.Headers;
                    }
                }
            }

            if (UseTransformer)
            {
                var factory = new HandlebarsContextFactory(settings.FileSystemHandler, settings.HandlebarsRegistrationCallback);
                var responseMessageTransformer = new ResponseMessageTransformer(factory);
                return responseMessageTransformer.Transform(requestMessage, responseMessage, UseTransformerForBodyAsFile);
            }

            if (!UseTransformer && ResponseMessage.BodyData?.BodyAsFileIsCached == true)
            {
                ResponseMessage.BodyData.BodyAsBytes = settings.FileSystemHandler.ReadResponseBodyAsFile(responseMessage.BodyData.BodyAsFile);
            }

            return responseMessage;
        }
    }
}
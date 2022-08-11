// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Proxy;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Transformers.Handlebars;
using WireMock.Transformers.Scriban;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The Response.
/// </summary>
public partial class Response : IResponseBuilder
{
    private static readonly ThreadLocal<Random> Random = new(() => new Random(DateTime.UtcNow.Millisecond));

    private TimeSpan? _delay;

    /// <summary>
    /// The minimum random delay in milliseconds.
    /// </summary>
    public int? MinimumDelayMilliseconds { get; private set; }

    /// <summary>
    /// The maximum random delay in milliseconds.
    /// </summary>
    public int? MaximumDelayMilliseconds { get; private set; }

    /// <summary>
    /// The delay
    /// </summary>
    public TimeSpan? Delay
    {
        get
        {
            if (MinimumDelayMilliseconds != null && MaximumDelayMilliseconds != null)
            {
                return TimeSpan.FromMilliseconds(Random.Value!.Next(MinimumDelayMilliseconds.Value, MaximumDelayMilliseconds.Value));
            }

            return _delay;
        }

        private set => _delay = value;
    }

    /// <summary>
    /// Gets a value indicating whether [use transformer].
    /// </summary>
    public bool UseTransformer { get; private set; }

    /// <summary>
    /// Gets the type of the transformer.
    /// </summary>
    public TransformerType TransformerType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether to use the Handlebars transformer for the content from the referenced BodyAsFile.
    /// </summary>
    public bool UseTransformerForBodyAsFile { get; private set; }

    /// <summary>
    /// Gets the ReplaceNodeOptions to use when transforming a JSON node.
    /// </summary>
    public ReplaceNodeOptions TransformerReplaceNodeOptions { get; private set; }

    /// <summary>
    /// Gets the response message.
    /// </summary>
    public ResponseMessage ResponseMessage { get; }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <param name="responseMessage">ResponseMessage</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    public static IResponseBuilder Create(ResponseMessage? responseMessage = null)
    {
        var message = responseMessage ?? new ResponseMessage();
        return new Response(message);
    }

    /// <summary>
    /// Creates this instance with the specified function.
    /// </summary>
    /// <param name="func">The callback function.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    public static IResponseBuilder Create(Func<ResponseMessage> func)
    {
        Guard.NotNull(func);

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
        Guard.NotNull(name);

        ResponseMessage.AddHeader(name, values);
        return this;
    }

    /// <inheritdoc cref="IHeadersResponseBuilder.WithHeaders(IDictionary{string, string})"/>
    public IResponseBuilder WithHeaders(IDictionary<string, string> headers)
    {
        Guard.NotNull(headers);

        ResponseMessage.Headers = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
        return this;
    }

    /// <inheritdoc cref="IHeadersResponseBuilder.WithHeaders(IDictionary{string, string[]})"/>
    public IResponseBuilder WithHeaders(IDictionary<string, string[]> headers)
    {
        Guard.NotNull(headers);

        ResponseMessage.Headers = headers.ToDictionary(header => header.Key, header => new WireMockList<string>(header.Value));
        return this;
    }

    /// <inheritdoc cref="IHeadersResponseBuilder.WithHeaders(IDictionary{string, WireMockList{string}})"/>
    public IResponseBuilder WithHeaders(IDictionary<string, WireMockList<string>> headers)
    {
        Guard.NotNull(headers);

        ResponseMessage.Headers = headers;
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(Func<IRequestMessage, string> bodyFactory, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(bodyFactory, nameof(bodyFactory));

        return WithCallbackInternal(true, req => new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = bodyFactory(req),
                Encoding = encoding ?? Encoding.UTF8
            }
        });
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(Func<IRequestMessage, Task<string>> bodyFactory, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(bodyFactory, nameof(bodyFactory));

        return WithCallbackInternal(true, async req => new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = await bodyFactory(req).ConfigureAwait(false),
                Encoding = encoding ?? Encoding.UTF8
            }
        });
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(byte[] body, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(body);

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
        Guard.NotNull(filename);

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

    /// <inheritdoc />
    public IResponseBuilder WithBody(string body, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(body);

        encoding ??= Encoding.UTF8;

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
    public IResponseBuilder WithBodyAsJson(object body, Encoding? encoding = null, bool? indented = null)
    {
        Guard.NotNull(body);

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
    public IResponseBuilder WithTransformer(bool transformContentFromBodyAsFile)
    {
        return WithTransformer(TransformerType.Handlebars, transformContentFromBodyAsFile);
    }

    /// <inheritdoc cref="ITransformResponseBuilder.WithTransformer(ReplaceNodeOptions)"/>
    public IResponseBuilder WithTransformer(ReplaceNodeOptions options)
    {
        return WithTransformer(TransformerType.Handlebars, false, options);
    }

    /// <inheritdoc />
    public IResponseBuilder WithTransformer(TransformerType transformerType, bool transformContentFromBodyAsFile = false, ReplaceNodeOptions options = ReplaceNodeOptions.None)
    {
        UseTransformer = true;
        TransformerType = transformerType;
        UseTransformerForBodyAsFile = transformContentFromBodyAsFile;
        TransformerReplaceNodeOptions = options;
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithDelay(TimeSpan delay)
    {
        Guard.Condition(delay, d => d == Timeout.InfiniteTimeSpan || d > TimeSpan.Zero);

        Delay = delay;
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithDelay(int milliseconds)
    {
        return WithDelay(TimeSpan.FromMilliseconds(milliseconds));
    }

    /// <inheritdoc />
    public IResponseBuilder WithRandomDelay(int minimumMilliseconds = 0, int maximumMilliseconds = 60_000)
    {
        Guard.Condition(minimumMilliseconds, min => min >= 0);
        Guard.Condition(maximumMilliseconds, max => max > minimumMilliseconds);

        MinimumDelayMilliseconds = minimumMilliseconds;
        MaximumDelayMilliseconds = maximumMilliseconds;

        return this;
    }

    /// <inheritdoc />
    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        Guard.NotNull(requestMessage);
        Guard.NotNull(settings);

        if (Delay != null)
        {
            await Task.Delay(Delay.Value).ConfigureAwait(false);
        }

        if (ProxyAndRecordSettings != null && _httpClientForProxy != null)
        {
            string RemoveFirstOccurrence(string source, string find)
            {
                int place = source.IndexOf(find, StringComparison.OrdinalIgnoreCase);
                return place >= 0 ? source.Remove(place, find.Length) : source;
            }

            var requestUri = new Uri(requestMessage.Url);

            // Build the proxy url and skip duplicates
            string extra = RemoveFirstOccurrence(requestUri.LocalPath.TrimEnd('/'), new Uri(ProxyAndRecordSettings.Url).LocalPath.TrimEnd('/'));
            requestMessage.ProxyUrl = ProxyAndRecordSettings.Url + extra + requestUri.Query;

            var proxyHelper = new ProxyHelper(settings);

            return await proxyHelper.SendAsync(
                ProxyAndRecordSettings,
                _httpClientForProxy,
                requestMessage,
                requestMessage.ProxyUrl
            ).ConfigureAwait(false);
        }

        ResponseMessage responseMessage;
        if (!WithCallbackUsed)
        {
            responseMessage = ResponseMessage;
        }
        else
        {
            if (Callback != null)
            {
                responseMessage = Callback(requestMessage);
            }
            else
            {
                responseMessage = await CallbackAsync!(requestMessage).ConfigureAwait(false);
            }

            // Copy StatusCode from ResponseMessage (if defined)
            if (ResponseMessage.StatusCode != null)
            {
                responseMessage.StatusCode = ResponseMessage.StatusCode;
            }

            // Copy Headers from ResponseMessage (if defined)
            if (ResponseMessage.Headers?.Count > 0)
            {
                responseMessage.Headers = ResponseMessage.Headers;
            }
        }

        if (UseTransformer)
        {
            ITransformer responseMessageTransformer;
            switch (TransformerType)
            {
                case TransformerType.Handlebars:
                    var factoryHandlebars = new HandlebarsContextFactory(settings.FileSystemHandler, settings.HandlebarsRegistrationCallback);
                    responseMessageTransformer = new Transformer(factoryHandlebars);
                    break;

                case TransformerType.Scriban:
                case TransformerType.ScribanDotLiquid:
                    var factoryDotLiquid = new ScribanContextFactory(settings.FileSystemHandler, TransformerType);
                    responseMessageTransformer = new Transformer(factoryDotLiquid);
                    break;

                default:
                    throw new NotImplementedException($"TransformerType '{TransformerType}' is not supported.");
            }

            return (responseMessageTransformer.Transform(requestMessage, responseMessage, UseTransformerForBodyAsFile, TransformerReplaceNodeOptions), null);
        }

        if (!UseTransformer && ResponseMessage.BodyData?.BodyAsFileIsCached == true)
        {
            ResponseMessage.BodyData.BodyAsBytes = settings.FileSystemHandler.ReadResponseBodyAsFile(responseMessage.BodyData!.BodyAsFile);
        }

        return (responseMessage, null);
    }
}
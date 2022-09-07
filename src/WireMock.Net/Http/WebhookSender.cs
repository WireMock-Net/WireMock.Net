using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Models;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Transformers.Handlebars;
using WireMock.Transformers.Scriban;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Http;

internal class WebhookSender
{
    private const string ClientIp = "::1";
    private static readonly HttpResponseMessage HttpResponseMessageOk = new()
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent("WebHook sent as Fire & Forget")
    };
    private static readonly ThreadLocal<Random> Random = new(() => new Random(DateTime.UtcNow.Millisecond));

    private readonly WireMockServerSettings _settings;

    public WebhookSender(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        IMapping mapping,
        IWebhookRequest webhookRequest,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage
    )
    {
        Guard.NotNull(client);
        Guard.NotNull(mapping);
        Guard.NotNull(webhookRequest);
        Guard.NotNull(originalRequestMessage);
        Guard.NotNull(originalResponseMessage);

        IBodyData? bodyData;
        IDictionary<string, WireMockList<string>>? headers;
        if (webhookRequest.UseTransformer == true)
        {
            ITransformer responseMessageTransformer;
            switch (webhookRequest.TransformerType)
            {
                case TransformerType.Handlebars:
                    var factoryHandlebars = new HandlebarsContextFactory(_settings.FileSystemHandler, _settings.HandlebarsRegistrationCallback);
                    responseMessageTransformer = new Transformer(factoryHandlebars);
                    break;

                case TransformerType.Scriban:
                case TransformerType.ScribanDotLiquid:
                    var factoryDotLiquid = new ScribanContextFactory(_settings.FileSystemHandler, webhookRequest.TransformerType);
                    responseMessageTransformer = new Transformer(factoryDotLiquid);
                    break;

                default:
                    throw new NotImplementedException($"TransformerType '{webhookRequest.TransformerType}' is not supported.");
            }

            (bodyData, headers) = responseMessageTransformer.Transform(mapping, originalRequestMessage, originalResponseMessage, webhookRequest.BodyData, webhookRequest.Headers, webhookRequest.TransformerReplaceNodeOptions);
        }
        else
        {
            bodyData = webhookRequest.BodyData;
            headers = webhookRequest.Headers;
        }

        // Create RequestMessage
        var requestMessage = new RequestMessage(
            new UrlDetails(webhookRequest.Url),
            webhookRequest.Method,
            ClientIp,
            bodyData,
            headers?.ToDictionary(x => x.Key, x => x.Value.ToArray())
        )
        {
            DateTime = DateTime.UtcNow
        };

        // Create HttpRequestMessage
        var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, webhookRequest.Url);

        // Delay (if required)
        if (TryGetDelay(webhookRequest, out var delay))
        {
            await Task.Delay(delay.Value).ConfigureAwait(false);
        }

        // Call the URL
        var sendTask = client.SendAsync(httpRequestMessage);

        return await SendAsync(sendTask);
    }

    private static async Task<HttpResponseMessage> SendAsync(Task<HttpResponseMessage> sendTask)
    {
        return await sendTask;
    }

    private static bool TryGetDelay(IWebhookRequest webhookRequest, [NotNullWhen(true)] out int? delay)
    {
        delay = webhookRequest.Delay;
        var minimumDelay = webhookRequest.MinimumRandomDelay;
        var maximumDelay = webhookRequest.MaximumRandomDelay;

        if (minimumDelay is not null && maximumDelay is not null && maximumDelay >= minimumDelay)
        {
            delay = Random.Value!.Next(minimumDelay.Value, maximumDelay.Value);
            return true;
        }

        return delay is not null;
    }
}
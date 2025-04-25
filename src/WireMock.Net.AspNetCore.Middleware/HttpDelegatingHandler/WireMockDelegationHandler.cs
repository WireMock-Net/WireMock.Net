using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stef.Validation;
using WireMock.Server;

namespace WireMock.Net.AspNetCore.Middleware.HttpDelegatingHandler;

/// <summary>
/// DelegatingHandler that takes requests made via the <see cref="HttpClient"/>
/// and routes them to the <see cref="WireMockServer"/>.
/// </summary>
internal class WireMockDelegationHandler : DelegatingHandler
{
    private readonly ILogger<WireMockDelegationHandler> _logger;
    private readonly WireMockServerInstance _server;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WireMockDelegationHandlerSettings _settings;

    /// <summary>
    /// Creates a new instance of <see cref="WireMockDelegationHandler"/>
    /// </summary>
    public WireMockDelegationHandler(
        ILogger<WireMockDelegationHandler> logger,
        WireMockServerInstance server,
        IHttpContextAccessor httpContextAccessor,
        WireMockDelegationHandlerSettings settings
    )
    {
        _server = Guard.NotNull(server);
        _httpContextAccessor = Guard.NotNull(httpContextAccessor);
        _logger = Guard.NotNull(logger);
        _settings = Guard.NotNull(settings);
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Guard.NotNull(request);

        if (_settings.AlwaysRedirect || IsWireMockRedirectHeaderSetToTrue())
        {
            _logger.LogDebug("Redirecting request to WireMock server");
            if (_server.Instance?.Url != null)
            {
                request.RequestUri = new Uri(_server.Instance.Url + request.RequestUri!.PathAndQuery);
            }
        }

        if (TryGetDelayHeaderValue(out var delayInMs))
        {
            await Task.Delay(delayInMs, cancellationToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private bool IsWireMockRedirectHeaderSetToTrue()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.LogDebug("HttpContext is not available in current runtime environment");
            return false;
        }

        return
            httpContext.Request.Headers.TryGetValue(AppConstants.HEADER_REDIRECT, out var values) &&
            bool.TryParse(values.ToString(), out var shouldRedirectToWireMock) && shouldRedirectToWireMock;
    }

    private bool TryGetDelayHeaderValue(out int delayInMs)
    {
        delayInMs = 0;
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.LogDebug("HttpContext is not available in current runtime environment");
            return false;
        }

        return
            httpContext.Request.Headers.TryGetValue(AppConstants.HEADER_RESPONSE_DELAY, out var values) &&
            int.TryParse(values.ToString(), out delayInMs);
    }
}
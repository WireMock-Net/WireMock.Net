using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.ResponseProviders;
using WireMock.Settings;

namespace WireMock;

/// <summary>
/// The IMapping interface.
/// </summary>
public interface IMapping
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// The datetime when this mapping was created or updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the TimeSettings (Start, End and TTL).
    /// </summary>
    ITimeSettings? TimeSettings { get; }

    /// <summary>
    /// Gets the unique title.
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// The full filename path for this mapping (only defined for static mappings).
    /// </summary>
    string? Path { get; set; }

    /// <summary>
    /// Gets the priority.  (A low value means higher priority.)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Scenario.
    /// </summary>
    string? Scenario { get; }

    /// <summary>
    /// Execution state condition for the current mapping.
    /// </summary>
    string? ExecutionConditionState { get; }

    /// <summary>
    /// The next state which will be signaled after the current mapping execution.
    /// In case the value is null, state will not be changed.
    /// </summary>
    string? NextState { get; }

    /// <summary>
    /// The number of times this match should be matched before the state will be changed to the next state.
    /// </summary>
    int? StateTimes { get; }

    /// <summary>
    /// The Request matcher.
    /// </summary>
    IRequestMatcher RequestMatcher { get; }

    /// <summary>
    /// The Provider.
    /// </summary>
    IResponseProvider Provider { get; }

    /// <summary>
    /// The WireMockServerSettings.
    /// </summary>
    WireMockServerSettings Settings { get; }

    /// <summary>
    /// Is State started ?
    /// </summary>
    bool IsStartState { get; }

    /// <summary>
    /// Gets a value indicating whether this mapping is an Admin Interface.
    /// </summary>
    /// <value>
    /// <c>true</c> if this mapping is an Admin Interface; otherwise, <c>false</c>.
    /// </value>
    bool IsAdminInterface { get; }

    /// <summary>
    /// Gets a value indicating whether this mapping is a Proxy Mapping.
    /// </summary>
    /// <value>
    /// <c>true</c> if this mapping is a Proxy Mapping; otherwise, <c>false</c>.
    /// </value>
    bool IsProxy { get; }

    /// <summary>
    /// Gets a value indicating whether this mapping to be logged.
    /// </summary>
    /// <value>
    /// <c>true</c> if this mapping to be logged; otherwise, <c>false</c>.
    /// </value>
    bool LogMapping { get; }

    /// <summary>
    /// The Webhooks.
    /// </summary>
    IWebhook[]? Webhooks { get; }

    /// <summary>
    /// Use Fire and Forget for the defined webhook(s). [Optional]
    /// </summary>
    bool? UseWebhooksFireAndForget { get; set; }

    /// <summary>
    /// Data Object which can be used when WithTransformer is used.
    /// e.g. lookup an path in this object using
    /// <example>
    /// Path.Lookup data "1"
    /// </example>
    /// </summary>
    object? Data { get; set; }

    /// <summary>
    /// ProvideResponseAsync
    /// </summary>
    /// <param name="requestMessage">The request message.</param>
    /// <returns>The <see cref="ResponseMessage"/> including a new (optional) <see cref="IMapping"/>.</returns>
    Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IRequestMessage requestMessage);

    /// <summary>
    /// Gets the RequestMatchResult based on the RequestMessage.
    /// </summary>
    /// <param name="requestMessage">The request message.</param>
    /// <param name="nextState">The Next State.</param>
    /// <returns>The <see cref="IRequestMatchResult"/>.</returns>
    IRequestMatchResult GetRequestMatchResult(IRequestMessage requestMessage, string? nextState);
}
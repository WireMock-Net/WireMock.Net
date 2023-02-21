using System;
using System.Collections.Generic;
using WireMock.Models;

namespace WireMock.Admin.Mappings;

/// <summary>
/// MappingModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class MappingModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid? Guid { get; set; }

    /// <summary>
    /// The datetime when this mapping was created or updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the TimeSettings when which this mapping should be used.
    /// </summary>
    public TimeSettingsModel? TimeSettings { get; set; }

    /// <summary>
    /// The unique title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The priority. (A low value means higher priority.)
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// The Scenario.
    /// </summary>
    public string? Scenario { get; set; }

    /// <summary>
    /// Execution state condition for the current mapping.
    /// </summary>
    public string? WhenStateIs { get; set; }

    /// <summary>
    /// The next state which will be signaled after the current mapping execution.
    /// In case the value is null state will not be changed.
    /// </summary>
    public string? SetStateTo { get; set; }

    /// <summary>
    /// The request model.
    /// </summary>
    public RequestModel Request { get; set; }

    /// <summary>
    /// The response model.
    /// </summary>
    public ResponseModel Response { get; set; }

    /// <summary>
    /// Saves this mapping as a static mapping file.
    /// </summary>
    public bool? SaveToFile { get; set; }

    /// <summary>
    /// The Webhook.
    /// </summary>
    public WebhookModel? Webhook { get; set; }

    /// <summary>
    /// The Webhooks.
    /// </summary>
    public WebhookModel[]? Webhooks { get; set; }

    /// <summary>
    /// Fire and forget for webhooks.
    /// </summary>
    public bool? UseWebhooksFireAndForget { get; set; }
    
    /// <summary>
    /// Data Dictionary Object which can be used when WithTransformer is used.
    /// e.g. lookup an element in this dictionary using
    /// <example>
    /// Dictionary.Lookup data "1"
    /// </example>
    /// </summary>
    public IDictionary<string, object?>? Data { get; set; }
}
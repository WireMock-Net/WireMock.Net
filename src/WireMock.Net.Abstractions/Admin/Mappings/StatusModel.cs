// Copyright © WireMock.Net

using System;

namespace WireMock.Admin.Mappings;

/// <summary>
/// Status
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class StatusModel
{
    /// <summary>
    /// The optional guid.
    /// </summary>
    public Guid? Guid { get; set; }

    /// <summary>
    /// The status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// The error message.
    /// </summary>
    public string? Error { get; set; }
}
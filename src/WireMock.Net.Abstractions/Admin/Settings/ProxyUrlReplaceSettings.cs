namespace WireMock.Admin.Settings;

/// <summary>
/// Defines an old path param and a new path param to be replaced when proxying.
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class ProxyUrlReplaceSettings
{
    /// <summary>
    /// The old path value to be replaced by the new path value
    /// </summary>
    public string OldValue { get; set; } = null!;

    /// <summary>
    /// The new path value to replace the old value with
    /// </summary>
    public string NewValue { get; set; } = null!;
}
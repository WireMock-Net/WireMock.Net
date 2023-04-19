namespace WireMock.Settings;

/// <summary>
/// Defines an old path param and a new path param to be replaced when proxying.
/// </summary>
public class ReplaceSettings
{
    /// <summary>
    /// The old path value to be replaced by the new path value
    /// </summary>
    public string OldValue = "";

    /// <summary>
    /// The new path value to replace the old value with
    /// </summary>
    public string NewValue = "";
}
// Copyright © WireMock.Net

using JetBrains.Annotations;
using WireMock.Types;

namespace WireMock.Settings;

/// <summary>
/// HandlebarsSettings
/// </summary>
[PublicAPI]
public class HandlebarsSettings
{
    /// <summary>
    /// Defines the allowed custom HandlebarHelpers which can be used. Possible values are:
    /// - <see cref="CustomHandlebarHelpers.None"/> (Default)
    /// - <see cref="CustomHandlebarHelpers.File"/>
    /// - <see cref="CustomHandlebarHelpers.All"/>
    /// </summary>
    [PublicAPI]
    public CustomHandlebarHelpers AllowedCustomHandlebarHelpers { get; set; } = CustomHandlebarHelpers.None;
}
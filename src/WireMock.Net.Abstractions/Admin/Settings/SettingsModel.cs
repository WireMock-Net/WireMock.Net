using System.Text.RegularExpressions;
using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Admin.Settings;

/// <summary>
/// Settings
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class SettingsModel
{
    /// <summary>
    /// Gets or sets the global delay in milliseconds.
    /// </summary>
    public int? GlobalProcessingDelay { get; set; }

    /// <summary>
    /// Gets or sets if partial mapping is allowed.
    /// </summary>
    public bool? AllowPartialMapping { get; set; }

    /// <summary>
    /// Gets or sets the RequestLog expiration in hours
    /// </summary>
    public int? RequestLogExpirationDuration { get; set; }

    /// <summary>
    /// Gets or sets the MaxRequestLog count.
    /// </summary>
    public int? MaxRequestLogCount { get; set; }

    /// <summary>
    /// Allow a Body for all HTTP Methods. (default set to false).
    /// </summary>
    public bool? AllowBodyForAllHttpMethods { get; set; }

    /// <summary>
    /// Handle all requests synchronously. (default set to false).
    /// </summary>
    public bool? HandleRequestsSynchronously { get; set; }

    /// <summary>
    /// Throw an exception when the Matcher fails because of invalid input. (default set to false).
    /// </summary>
    public bool? ThrowExceptionWhenMatcherFails { get; set; }

    /// <summary>
    /// Use the RegexExtended instead of the default <see cref="Regex"/>.  (default set to true).
    /// </summary>
    public bool? UseRegexExtended { get; set; }

    /// <summary>
    /// Save unmatched requests to a file using the <see cref="IFileSystemHandler"/>. (default set to false).
    /// </summary>
    public bool? SaveUnmatchedRequests { get; set; }

    /// <summary>
    /// Gets or sets if the static mappings should be read at startup.
    /// </summary>
    public bool? ReadStaticMappings { get; set; }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    /// </summary>
    public bool? WatchStaticMappings { get; set; }

    /// <summary>
    /// A value indicating whether subdirectories within the static mappings path should be monitored.
    /// </summary>
    public bool? WatchStaticMappingsInSubdirectories { get; set; }

    /// <summary>
    /// Policies to use when using CORS. By default CORS is disabled. [Optional]
    /// </summary>
    public string? CorsPolicyOptions { get; set; }

    /// <summary>
    /// The proxy and record settings.
    /// </summary>
    public ProxyAndRecordSettingsModel? ProxyAndRecordSettings { get; set; }

    /// <summary>
    /// Defines on which scheme (http/https) to host. (This overrides the <c>UseSSL</c> value).
    /// </summary>
    public HostingScheme? HostingScheme { get; set; }

    /// <summary>
    /// Don't save the response-string in the LogEntry when WithBody(Func{IRequestMessage, string}) or WithBody(Func{IRequestMessage, Task{string}}) is used. (default set to false).
    /// </summary>
    public bool? DoNotSaveDynamicResponseInLogEntry { get; set; }
}
using System;

namespace WireMock.Settings
{
    /// <summary>
    /// IFluentMockServerSettings
    /// </summary>
    public interface IFluentMockServerSettings
    {
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        int? Port { get; set; }

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        bool? UseSSL { get; set; }

        /// <summary>
        /// Gets or sets wether to start admin interface.
        /// </summary>
        bool? StartAdminInterface { get; set; }

        /// <summary>
        /// Gets or sets if the static mappings should be read at startup.
        /// </summary>
        bool? ReadStaticMappings { get; set; }

        /// <summary>
        /// Gets or sets if the proxy and record settings.
        /// </summary>
        IProxyAndRecordSettings ProxyAndRecordSettings { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        string[] Urls { get; set; }

        /// <summary>
        /// StartTimeout
        /// </summary>
        int StartTimeout { get; set; }

        /// <summary>
        /// Allow Partial Mapping (default set to false).
        /// </summary>
        bool? AllowPartialMapping { get; set; }

        /// <summary>
        /// The username needed for __admin access.
        /// </summary>
        string AdminUsername { get; set; }

        /// <summary>
        /// The password needed for __admin access.
        /// </summary>
        string AdminPassword { get; set; }

        /// <summary>
        /// The RequestLog expiration in hours (optional).
        /// </summary>
        int? RequestLogExpirationDuration { get; set; }

        /// <summary>
        /// The MaxRequestLog count (optional).
        /// </summary>
        int? MaxRequestLogCount { get; set; }

        /// <summary>
        /// Action which is called (with the IAppBuilder or IApplicationBuilder) before the internal WireMockMiddleware is initialized. [Optional]
        /// </summary>
        Action<object> PreWireMockMiddlewareInit { get; set; }

        /// <summary>
        /// Action which is called (with the IAppBuilder or IApplicationBuilder) after the internal WireMockMiddleware is initialized. [Optional]
        /// </summary>
        Action<object> PostWireMockMiddlewareInit { get; set; }
    }
}
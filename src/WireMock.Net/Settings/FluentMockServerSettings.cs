using System;
using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// FluentMockServerSettings
    /// </summary>
    public class FluentMockServerSettings
    {
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [PublicAPI]
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        [PublicAPI]
        public bool? UseSSL { get; set; }

        /// <summary>
        /// Gets or sets wether to start admin interface.
        /// </summary>
        [PublicAPI]
        public bool? StartAdminInterface { get; set; }

        /// <summary>
        /// Gets or sets if the static mappings should be read at startup.
        /// </summary>
        [PublicAPI]
        public bool? ReadStaticMappings { get; set; }

        /// <summary>
        /// Gets or sets if the server should record and save requests and responses.
        /// </summary>
        /// <value>true/false</value>
        [PublicAPI]
        public ProxyAndRecordSettings ProxyAndRecordSettings { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        [PublicAPI]
        public string[] Urls { get; set; }

        /// <summary>
        /// StartTimeout
        /// </summary>
        [PublicAPI]
        public int StartTimeout { get; set; } = 10000;

        /// <summary>
        /// Allow Partial Mapping (default set to false).
        /// </summary>
        [PublicAPI]
        public bool? AllowPartialMapping { get; set; }

        /// <summary>
        /// The username needed for __admin access.
        /// </summary>
        [PublicAPI]
        public string AdminUsername { get; set; }

        /// <summary>
        /// The password needed for __admin access.
        /// </summary>
        [PublicAPI]
        public string AdminPassword { get; set; }

        /// <summary>
        /// The RequestLog expiration in hours (optional).
        /// </summary>
        [PublicAPI]
        public int? RequestLogExpirationDuration { get; set; }

        /// <summary>
        /// The MaxRequestLog count (optional).
        /// </summary>
        [PublicAPI]
        public int? MaxRequestLogCount { get; set; }

        /// <summary>
        /// Action which is called (with the IAppBuilder or IApplicationBuilder) before the internal WireMockMiddleware is initialized. [Optional]
        /// </summary>
        [PublicAPI]
        public Action<object> PreWireMockMiddlewareInit { get; set; }

        /// <summary>
        /// Action which is called (with the IAppBuilder or IApplicationBuilder) after the internal WireMockMiddleware is initialized. [Optional]
        /// </summary>
        [PublicAPI]
        public Action<object> PostWireMockMiddlewareInit { get; set; }
    }
}
using System;
using HandlebarsDotNet;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Handlers;
using WireMock.Logging;

namespace WireMock.Settings
{
    /// <summary>
    /// WireMockServerSettings
    /// </summary>
    public class WireMockServerSettings
    {
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [PublicAPI]
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        [PublicAPI]
        public string[] Urls { get; set; }

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        public bool? UseSSL { get; set; }

        /// <summary>
        /// Gets or sets whether to start admin interface.
        /// </summary>
        [PublicAPI]
        public bool? StartAdminInterface { get; set; }

        /// <summary>
        /// Gets or sets if the static mappings should be read at startup.
        /// </summary>
        [PublicAPI]
        public bool? ReadStaticMappings { get; set; }

        /// <summary>
        /// Watch the static mapping files + folder for changes when running.
        /// </summary>
        [PublicAPI]
        public bool? WatchStaticMappings { get; set; }

        /// <summary>
        /// Gets or sets if the proxy and record settings.
        /// </summary>
        [PublicAPI]
        public ProxyAndRecordSettings ProxyAndRecordSettings { get; set; }

        /// <summary>
        /// StartTimeout in milliseconds.
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
        [JsonIgnore]
        public Action<object> PreWireMockMiddlewareInit { get; set; }

        /// <summary>
        /// Action which is called (with the IAppBuilder or IApplicationBuilder) after the internal WireMockMiddleware is initialized. [Optional]
        /// </summary>
        [PublicAPI]
        [JsonIgnore]
        public Action<object> PostWireMockMiddlewareInit { get; set; }

        /// <summary>
        /// The IWireMockLogger which logs Debug, Info, Warning or Error
        /// </summary>
        [PublicAPI]
        [JsonIgnore]
        public IWireMockLogger Logger { get; set; } = new WireMockNullLogger();

        /// <summary>
        /// Handler to interact with the file system to read and write static mapping files.
        /// </summary>
        [PublicAPI]
        [JsonIgnore]
        public IFileSystemHandler FileSystemHandler { get; set; }

        /// <summary>
        /// Action which can be used to add additional Handlebars registrations. [Optional]
        /// </summary>
        [PublicAPI]
        public Action<IHandlebars, IFileSystemHandler> HandlebarsRegistrationCallback { get; set; }

        /// <summary>
        /// Allow the usage of CSharpCodeMatcher (default is not allowed).
        /// </summary>
        [PublicAPI]
        public bool? AllowCSharpCodeMatcher { get; set; }
    }
}
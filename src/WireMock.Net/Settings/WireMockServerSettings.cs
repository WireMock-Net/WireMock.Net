using HandlebarsDotNet;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using WireMock.Handlers;
using WireMock.Logging;

namespace WireMock.Settings
{
    /// <summary>
    /// WireMockServerSettings
    /// </summary>
    public class WireMockServerSettings : IWireMockServerSettings
    {
        /// <inheritdoc cref="IWireMockServerSettings.Port"/>
        [PublicAPI]
        public int? Port { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.UseSSL"/>
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        public bool? UseSSL { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.StartAdminInterface"/>
        [PublicAPI]
        public bool? StartAdminInterface { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.ReadStaticMappings"/>
        [PublicAPI]
        public bool? ReadStaticMappings { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.WatchStaticMappings"/>
        [PublicAPI]
        public bool? WatchStaticMappings { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.ProxyAndRecordSettings"/>
        [PublicAPI]
        public IProxyAndRecordSettings ProxyAndRecordSettings { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.Urls"/>
        [PublicAPI]
        public string[] Urls { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.StartTimeout"/>
        [PublicAPI]
        public int StartTimeout { get; set; } = 10000;

        /// <inheritdoc cref="IWireMockServerSettings.AllowPartialMapping"/>
        [PublicAPI]
        public bool? AllowPartialMapping { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.AdminUsername"/>
        [PublicAPI]
        public string AdminUsername { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.AdminPassword"/>
        [PublicAPI]
        public string AdminPassword { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.RequestLogExpirationDuration"/>
        [PublicAPI]
        public int? RequestLogExpirationDuration { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.MaxRequestLogCount"/>
        [PublicAPI]
        public int? MaxRequestLogCount { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.PreWireMockMiddlewareInit"/>
        [PublicAPI]
        [JsonIgnore]
        public Action<object> PreWireMockMiddlewareInit { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.PostWireMockMiddlewareInit"/>
        [PublicAPI]
        [JsonIgnore]
        public Action<object> PostWireMockMiddlewareInit { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.Logger"/>
        [PublicAPI]
        [JsonIgnore]
        public IWireMockLogger Logger { get; set; } = new WireMockNullLogger();

        /// <inheritdoc cref="IWireMockServerSettings.FileSystemHandler"/>
        [PublicAPI]
        [JsonIgnore]
        public IFileSystemHandler FileSystemHandler { get; set; }

        /// <inheritdoc cref="IWireMockServerSettings.HandlebarsRegistrationCallback"/>
        [PublicAPI]
        [JsonIgnore]
        public Action<IHandlebars, IFileSystemHandler> HandlebarsRegistrationCallback { get; set; }
    }
}
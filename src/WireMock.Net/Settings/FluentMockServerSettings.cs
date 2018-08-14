using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Handlers;
using WireMock.Logging;

namespace WireMock.Settings
{
    /// <summary>
    /// FluentMockServerSettings
    /// </summary>
    public class FluentMockServerSettings : IFluentMockServerSettings
    {
        /// <inheritdoc cref="IFluentMockServerSettings.Port"/>
        [PublicAPI]
        public int? Port { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.UseSSL"/>
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        public bool? UseSSL { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.StartAdminInterface"/>
        [PublicAPI]
        public bool? StartAdminInterface { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.ReadStaticMappings"/>
        [PublicAPI]
        public bool? ReadStaticMappings { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.WatchStaticMappings"/>
        [PublicAPI]
        public bool? WatchStaticMappings { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.ProxyAndRecordSettings"/>
        [PublicAPI]
        public IProxyAndRecordSettings ProxyAndRecordSettings { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.Urls"/>
        [PublicAPI]
        public string[] Urls { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.StartTimeout"/>
        [PublicAPI]
        public int StartTimeout { get; set; } = 10000;

        /// <inheritdoc cref="IFluentMockServerSettings.AllowPartialMapping"/>
        [PublicAPI]
        public bool? AllowPartialMapping { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.AdminUsername"/>
        [PublicAPI]
        public string AdminUsername { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.AdminPassword"/>
        [PublicAPI]
        public string AdminPassword { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.RequestLogExpirationDuration"/>
        [PublicAPI]
        public int? RequestLogExpirationDuration { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.MaxRequestLogCount"/>
        [PublicAPI]
        public int? MaxRequestLogCount { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.PreWireMockMiddlewareInit"/>
        [PublicAPI]
        [JsonIgnore]
        public Action<object> PreWireMockMiddlewareInit { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.PostWireMockMiddlewareInit"/>
        [PublicAPI]
        [JsonIgnore]
        public Action<object> PostWireMockMiddlewareInit { get; set; }

        /// <inheritdoc cref="IFluentMockServerSettings.Logger"/>
        [PublicAPI]
        [JsonIgnore]
        public IWireMockLogger Logger { get; set; } = new WireMockNullLogger();

        /// <inheritdoc cref="IFluentMockServerSettings.FileSystemHandler"/>
        [PublicAPI]
        [JsonIgnore]
        public IFileSystemHandler FileSystemHandler { get; set; } = new LocalFileSystemHandler();
    }
}
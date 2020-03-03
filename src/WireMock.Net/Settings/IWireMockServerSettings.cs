﻿using System;
using HandlebarsDotNet;
using JetBrains.Annotations;
using WireMock.Handlers;
using WireMock.Logging;

namespace WireMock.Settings
{
    /// <summary>
    /// IWireMockServerSettings
    /// </summary>
    public interface IWireMockServerSettings
    {
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        [PublicAPI]
        int? Port { get; set; }

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        [PublicAPI]
        bool? UseSSL { get; set; }

        /// <summary>
        /// Gets or sets whether to start admin interface.
        /// </summary>
        [PublicAPI]
        bool? StartAdminInterface { get; set; }

        /// <summary>
        /// Gets or sets if the static mappings should be read at startup.
        /// </summary>
        [PublicAPI]
        bool? ReadStaticMappings { get; set; }

        /// <summary>
        /// Watch the static mapping files + folder for changes when running.
        /// </summary>
        [PublicAPI]
        bool? WatchStaticMappings { get; set; }

        /// <summary>
        /// A value indicating whether subdirectories within the static mappings path should be monitored.
        /// </summary>
        [PublicAPI]
        bool? WatchStaticMappingsInSubdirectories { get; set; }

        /// <summary>
        /// Gets or sets if the proxy and record settings.
        /// </summary>
        [PublicAPI]
        IProxyAndRecordSettings ProxyAndRecordSettings { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        [PublicAPI]
        string[] Urls { get; set; }

        /// <summary>
        /// StartTimeout
        /// </summary>
        [PublicAPI]
        int StartTimeout { get; set; }

        /// <summary>
        /// Allow Partial Mapping (default set to false).
        /// </summary>
        [PublicAPI]
        bool? AllowPartialMapping { get; set; }

        /// <summary>
        /// The username needed for __admin access.
        /// </summary>
        [PublicAPI]
        string AdminUsername { get; set; }

        /// <summary>
        /// The password needed for __admin access.
        /// </summary>
        [PublicAPI]
        string AdminPassword { get; set; }

        /// <summary>
        /// The RequestLog expiration in hours (optional).
        /// </summary>
        [PublicAPI]
        int? RequestLogExpirationDuration { get; set; }

        /// <summary>
        /// The MaxRequestLog count (optional).
        /// </summary>
        [PublicAPI]
        int? MaxRequestLogCount { get; set; }

        /// <summary>
        /// Action which is called (with the IAppBuilder or IApplicationBuilder) before the internal WireMockMiddleware is initialized. [Optional]
        /// </summary>
        [PublicAPI]
        Action<object> PreWireMockMiddlewareInit { get; set; }

        /// <summary>
        /// Action which is called (with the IAppBuilder or IApplicationBuilder) after the internal WireMockMiddleware is initialized. [Optional]
        /// </summary>
        [PublicAPI]
        Action<object> PostWireMockMiddlewareInit { get; set; }

        /// <summary>
        /// The IWireMockLogger which logs Debug, Info, Warning or Error
        /// </summary>
        [PublicAPI]
        IWireMockLogger Logger { get; set; }

        /// <summary>
        /// Handler to interact with the file system to read and write static mapping files.
        /// </summary>
        [PublicAPI]
        IFileSystemHandler FileSystemHandler { get; set; }

        /// <summary>
        /// Action which can be used to add additional Handlebars registrations. [Optional]
        /// </summary>
        [PublicAPI]
        Action<IHandlebars, IFileSystemHandler> HandlebarsRegistrationCallback { get; set; }

        /// <summary>
        /// Allow the usage of CSharpCodeMatcher (default is not allowed).
        /// </summary>
        [PublicAPI]
        bool? AllowCSharpCodeMatcher { get; set; }

        /// <summary>
        /// Allow a Body for all HTTP Methods. (default set to false).
        /// </summary>
        [PublicAPI]
        bool? AllowBodyForAllHttpMethods { get; set; }

        /// <summary>
        /// Allow any HttpStatusCode in the response. Also null, 0, empty or invalid. (default set to false).
        /// </summary>
        /// [PublicAPI]
        bool? AllowAnyHttpStatusCodeInResponse { get; set; }

        /// <summary>
        /// Set to true to disable Json deserialization when processing requests. (default set to false).
        /// </summary>
        [PublicAPI]
        bool? DisableJsonBodyParsing { get; set; }
    }
}
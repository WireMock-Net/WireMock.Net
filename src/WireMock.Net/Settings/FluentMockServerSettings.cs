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
        /// <value>
        /// The port.
        /// </value>
        public int? Port { get; set; }

        /// <summary>
        /// Gets or sets the use SSL.
        /// </summary>
        /// <value>
        /// The use SSL.
        /// </value>
        // ReSharper disable once InconsistentNaming
        public bool? UseSSL { get; set; }

        /// <summary>
        /// Gets or sets the start admin interface.
        /// </summary>
        /// <value>
        /// The start admin interface.
        /// </value>
        public bool? StartAdminInterface { get; set; }

        /// <summary>
        /// Gets or sets if the static mappings should be read at startup.
        /// </summary>
        /// <value>true/false</value>
        public bool? ReadStaticMappings { get; set; }

        /// <summary>
        /// Gets or sets if the server should record and save requests and responses.
        /// </summary>
        /// <value>true/false</value>
        public ProxyAndRecordSettings ProxyAndRecordSettings { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        /// <value>
        /// The urls.
        /// </value>
        public string[] Urls { get; set; }

        /// <summary>
        /// StartTimeout
        /// </summary>
        public int StartTimeout { get; set; } = 10000;
    }
}
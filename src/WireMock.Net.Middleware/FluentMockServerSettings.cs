namespace WireMock.Net
{
    /// <summary>
    /// WireMockSettings
    /// </summary>
    public class WireMockSettings
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
        /// Gets or sets the read static mappings.
        /// </summary>
        /// <value>
        /// The read static mappings.
        /// </value>
        public bool? ReadStaticMappings { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        /// <value>
        /// The urls.
        /// </value>
        public string[] Urls { get; set; }
    }
}
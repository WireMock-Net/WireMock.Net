namespace WireMock.Admin.Settings
{
    /// <summary>
    /// Settings
    /// </summary>
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
    }
}
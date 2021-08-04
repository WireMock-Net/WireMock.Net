namespace WireMock.Admin.Settings
{
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
    }
}
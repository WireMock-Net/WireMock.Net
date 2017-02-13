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
    }
}
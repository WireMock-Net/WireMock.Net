namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// Fault Model
    /// </summary>
#if !NET45
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class FaultModel
    {
        /// <summary>
        /// Gets or sets the fault. Can be null, "", NONE, EMPTY_RESPONSE, MALFORMED_RESPONSE_CHUNK or RANDOM_DATA_THEN_CLOSE.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the fault percentage.
        /// </summary>
        public double? Percentage { get; set; }
    }
}
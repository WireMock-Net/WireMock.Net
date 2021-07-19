namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// EncodingModel
    /// </summary>
#if !NET45
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class EncodingModel
    {
        /// <summary>
        /// Encoding CodePage
        /// </summary>
        public int CodePage { get; set; }

        /// <summary>
        /// Encoding EncodingName
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Encoding WebName
        /// </summary>
        public string WebName { get; set; }
    }
}
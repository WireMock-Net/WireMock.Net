namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// ClientIPModel
    /// </summary>
#if RESTCLIENT
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class ClientIPModel
    {
        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}
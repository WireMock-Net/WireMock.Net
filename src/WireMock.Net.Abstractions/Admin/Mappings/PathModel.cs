namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// PathModel
    /// </summary>
#if RESTCLIENT
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class PathModel
    {
        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}
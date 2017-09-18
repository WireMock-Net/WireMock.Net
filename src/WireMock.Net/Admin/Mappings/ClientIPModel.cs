namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// ClientIPModel
    /// </summary>
    public class ClientIPModel
    {
        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        /// <value>
        /// The matchers.
        /// </value>
        public MatcherModel[] Matchers { get; set; }

        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        public string[] Funcs { get; set; }
    }
}
namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// Body Model
    /// </summary>
    public class BodyModel
    {
        /// <summary>
        /// Gets or sets the matcher.
        /// </summary>
        /// <value>
        /// The matcher.
        /// </value>
        public MatcherModel Matcher { get; set; }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        public string Func { get; set; }

        /// <summary>
        /// Gets or sets the data function.
        /// </summary>
        /// <value>
        /// The data function.
        /// </value>
        public string DataFunc { get; set; }
    }
}
namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// MatcherModel
    /// </summary>
    public class MatcherModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the patterns.
        /// </summary>
        /// <value>
        /// The patterns.
        /// </value>
        public string[] Patterns { get; set; }

        /// <summary>
        /// Gets or sets the ignore case.
        /// </summary>
        /// <value>
        /// The ignore case.
        /// </value>
        public bool? IgnoreCase { get; set; }
    }
}

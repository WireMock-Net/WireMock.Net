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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the patterns.
        /// </summary>
        public string[] Patterns { get; set; }

        /// <summary>
        /// Gets or sets the ignore case.
        /// </summary>
        public bool? IgnoreCase { get; set; }

        /// <summary>
        /// Reject on match.
        /// </summary>
        public bool? RejectOnMatch { get; set; }
    }
}

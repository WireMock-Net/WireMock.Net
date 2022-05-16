namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// MatcherModel
    /// </summary>
    [FluentBuilder.AutoGenerateBuilder]
    public class MatcherModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the pattern. Can be a string (default) or an object.
        /// </summary>
        public object? Pattern { get; set; }

        /// <summary>
        /// Gets or sets the patterns. Can be array of strings (default) or an array of objects.
        /// </summary>
        public object[]? Patterns { get; set; }

        /// <summary>
        /// Gets or sets the pattern as a file.
        /// </summary>
        public string? PatternAsFile { get; set; }

        /// <summary>
        /// Gets or sets the ignore case.
        /// </summary>
        public bool? IgnoreCase { get; set; }

        /// <summary>
        /// Reject on match.
        /// </summary>
        public bool? RejectOnMatch { get; set; }

        /// <summary>
        /// The Operator to use when multiple patterns are defined. Optional.
        /// - null      = Same as "or".
        /// - "or"      = Only one pattern should match.
        /// - "and"     = All patterns should match.
        /// - "average" = The average value from all patterns.
        /// </summary>
        public string? MatchOperator { get; set; }
    }
}
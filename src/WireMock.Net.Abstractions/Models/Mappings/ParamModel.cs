namespace WireMock.Models.Mappings
{
    /// <summary>
    /// Param Model
    /// </summary>
    public class ParamModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines if the key should be matched using case-ignore.
        /// </summary>
        public bool? IgnoreCase { get; set; }

        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}